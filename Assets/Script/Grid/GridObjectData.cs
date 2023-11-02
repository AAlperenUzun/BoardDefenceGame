using System;
using UnityEngine;

public class GridObjectData
{
    public Vector2Int Position;
    public GridObjectTypeContainer TypeContainer;
    public GridObjectState ObjectState;
    public readonly Axis Axis;

    public static GridObjectData CreateCube(Vector2Int position, GridCubeType cubeType, GridDefenceItemType defenceItemType, GridEnemyType enemyType)
    {
        return new GridObjectData(position, GridObjectType.Cube, cubeType, defenceItemType, enemyType, GridObjectState.Idle, Axis.None);
    }

    public static GridObjectData Create(Vector2Int position, GridObjectType objectType, Axis axis)
    {
        return new GridObjectData(position, objectType, GridCubeType.Invalid, GridDefenceItemType.Invalid, GridEnemyType.Invalid, GridObjectState.Idle, axis);
    }

    public GridObjectData(Vector2Int position, GridObjectType objectType, GridCubeType cubeType, GridDefenceItemType defenceItemType, GridEnemyType enemyType, GridObjectState objectState, Axis axis)
    {
        Position = position;
        TypeContainer = new GridObjectTypeContainer(objectType, cubeType, defenceItemType, enemyType);
        ObjectState = objectState;
        Axis = axis;
    }
}

[Serializable]
public struct GridObjectTypeContainer : IEquatable<GridObjectTypeContainer>
{
    public GridObjectType GridObjectType;
    public GridCubeType GridCubeType;
    public GridDefenceItemType GridDefenceItemType;
    public GridEnemyType GridEnemyType;

    public bool IsCube => GridObjectType == GridObjectType.Cube;

    public static readonly GridObjectTypeContainer Invalid = new GridObjectTypeContainer(GridObjectType.Invalid, GridCubeType.Invalid, GridDefenceItemType.Invalid, GridEnemyType.Invalid);
    
    public GridObjectTypeContainer(GridObjectType gridObjectType, GridCubeType gridCubeType, GridDefenceItemType gridDefenceItemType, GridEnemyType gridEnemyType)
    {
        GridObjectType = gridObjectType;
        GridCubeType = gridCubeType;
        GridDefenceItemType = gridDefenceItemType;
        GridEnemyType = gridEnemyType;
    }

    public bool IsValid()
    {
        if (GridObjectType == GridObjectType.Invalid)
            return false;
        
        if (GridObjectType == GridObjectType.Cube && GridCubeType == GridCubeType.Invalid) 
            return false;

        return true;
    }
    
    public static bool operator ==(GridObjectTypeContainer lhs, GridObjectTypeContainer rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(GridObjectTypeContainer lhs, GridObjectTypeContainer rhs)
    {
        return !lhs.Equals(rhs);
    }

    public bool Equals(GridObjectTypeContainer other)
    {
        if (GridObjectType == other.GridObjectType && IsCube)
        {
            return GridCubeType == other.GridCubeType;
        }
        
        return GridObjectType == other.GridObjectType;
    }

    public override bool Equals(object obj)
    {
        return obj is GridObjectTypeContainer other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)GridObjectType, (int)GridCubeType);
    }
}

public enum Axis : byte
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    Both = 3
}

public enum Direction : byte
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4
}