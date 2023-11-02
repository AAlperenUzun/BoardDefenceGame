using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Grid
{
    private readonly byte _rows;
    private readonly byte _columns;
    private readonly List<Vector2Int> _connectedPoints = new List<Vector2Int>();

    public BaseGridObjectController[] GridObjects { get; private set; }

    public int TopX { get; }
    public int TopY { get; }

    public Grid(byte rows, byte columns)
    {
        _rows = rows;
        _columns = columns;
        TopY = rows - 1;
        TopX = columns - 1;

        GridObjects = new BaseGridObjectController[columns * rows];
    }

    public void Update(float deltaTime)
    {
        for (var i = 0; i < GridObjects.Length; i++)
        {
            if (GridObjects[i] == null)
                continue;
            GridObjects[i].Update(deltaTime);
        }
    }

    public bool TryGetGridObject(int x, int y, out BaseGridObjectController data)
    {
        data = null;

        if (x >= _columns || x < 0)
            return false;

        if (y >= _rows || y < 0)
            return false;

        data = GridObjects[ToSingleDimensionIndex(x, y)];

        return true;
    }

    public void SetGridObject(BaseGridObjectController controller, int x, int y)
    {
        int index = ToSingleDimensionIndex(x, y);

        if (index >= GridObjects.Length || index < 0)
            throw new Exception("Object is out of bounds");

        GridObjects[index] = controller;
    }

    public void SetGridObject(BaseGridObjectController controller)
    {
        SetGridObject(controller, controller.GetPosition().x, controller.GetPosition().y);
    }

    public List<Vector2Int> GetObjectsInAxis(Vector2Int origin, Axis axis)
    {
        _connectedPoints.Clear();

        if (axis is Axis.Horizontal or Axis.Both)
            for (int i = 0; i < _columns; i++)
            {
                _connectedPoints.Add(new Vector2Int(i, origin.y));
            }
        else if (axis is Axis.Vertical or Axis.Both)
            for (int i = 0; i < _rows; i++)
            {
                _connectedPoints.Add(new Vector2Int(origin.x, i));
            }

        return _connectedPoints;
    }

    public void UpdatePositionsToFillSpaces()
    {
        NativeArray<GridObjectPresenterData> grid = new NativeArray<GridObjectPresenterData>(GridObjects.Length, Allocator.TempJob);
        NativeArray<Vector2Int> positions = new NativeArray<Vector2Int>(GridObjects.Length, Allocator.TempJob);
        NativeArray<bool> isChanged = new NativeArray<bool>(grid.Length, Allocator.TempJob);

        for (int i = 0; i < GridObjects.Length; i++)
        {
            if (GridObjects[i] == null)
            {
                grid[i] = GridObjectPresenterData.Invalid;
                positions[i] = Vector2Int.zero;
                continue;
            }

            grid[i] = GridObjects[i].GetPresenterData();
            positions[i] = GridObjects[i].GetPosition();
        }

        FillEmptySpacesJob job = new FillEmptySpacesJob()
        {
            Grid = grid,
            Rows = _rows,
            Columns = _columns,
            NewPositions = positions,
            IsChanged = isChanged,
        };

        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();

        BaseGridObjectController[] copy = new BaseGridObjectController[GridObjects.Length];

        for (int i = 0; i < GridObjects.Length; i++)
        {
            if (isChanged[i])
            {
                copy[ToSingleDimensionIndex(positions[i].x, positions[i].y)] = GridObjects[i];
            }
            else
            {
                copy[i] = GridObjects[i];
            }
        }

        GridObjects = copy;

        grid.Dispose();
        positions.Dispose();
        isChanged.Dispose();
    }

    public IEnumerable<(BaseGridObjectController, Vector2Int)> GetUpdatedPositions()
    {
        for (var i = 0; i < GridObjects.Length; i++)
        {
            if (GridObjects[i] == null)
                continue;

            Vector2Int correctPosition = ToTwoDimensionIndex(i);

            if (correctPosition != GridObjects[i].GetPosition())
            {
                yield return (GridObjects[i], correctPosition);
            }
        }
    }

    public int GetAvailableEmptyYPosition(int x)
    {
        int row = TopY;

        for (int y = TopY; y >= 0; y--)
        {
            if (GridObjects[ToSingleDimensionIndex(x, y)] == null)
            {
                row = y;
            }
        }

        return row;
    }

    public int ToSingleDimensionIndex(int x, int y)
    {
        return x + y * _columns;
    }

    public Vector2Int ToTwoDimensionIndex(int index)
    {
        return new Vector2Int(index % _columns, index / _columns);
    }
}

[BurstCompile(CompileSynchronously = true, FloatPrecision = FloatPrecision.Low, FloatMode = FloatMode.Fast,
    OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = false)]
public struct FindConnectedObjectsJob : IJob
{
    [ReadOnly] public NativeArray<GridObjectPresenterData> Grid;
    [ReadOnly] public byte Rows;
    [ReadOnly] public byte Columns;

    public NativeArray<bool> Visited;
    public NativeArray<bool> IsConnected;
    public NativeQueue<int> Queue;

    public void Execute()
    {
        while (Queue.Count > 0)
        {
            int current = Queue.Dequeue();
            int currentRow = current / Columns;
            int currentCol = current % Columns;

            for (int i = 0; i < GameUtility.DirectionCount; i++)
            {
                int newRow = currentRow + GameUtility.GetRowDirection(i);
                int newCol = currentCol + GameUtility.GetColDirection(i);
                int index = newRow * Columns + newCol;

                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns &&
                    !Visited[index])
                {
                    Visited[index] = true;

                    if (GameUtility.IsMatching(Grid[current], Grid[index]))
                    {
                        Queue.Enqueue(index);
                        IsConnected[index] = true;
                    }
                }
            }
        }
    }
}


[BurstCompile(CompileSynchronously = true, FloatPrecision = FloatPrecision.Low, FloatMode = FloatMode.Fast,
    OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = false)]
public struct FillEmptySpacesJob : IJob
{
    [ReadOnly] public NativeArray<GridObjectPresenterData> Grid;
    [ReadOnly] public byte Rows;
    [ReadOnly] public byte Columns;

    public NativeArray<Vector2Int> NewPositions;
    public NativeArray<bool> IsChanged;

    public void Execute()
    {
        for (int i = 0; i < Grid.Length; i++)
        {
            if (Grid[i].IsValid)
                continue;

            int currentRow = i / Columns;
            int currentCol = i % Columns;

            for (int j = currentRow; j < Rows; j++)
            {
                int index = j * Columns + currentCol;

                if (Grid[index].IsValid)
                {
                    Vector2Int oldPosition = NewPositions[index];
                    oldPosition.y--;
                    NewPositions[index] = oldPosition;
                    IsChanged[index] = true;
                }
            }
        }
    }
}