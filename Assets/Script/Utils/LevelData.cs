using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public event Action MoveUsed;
    public event Action GoalProgressed;

    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Vector2Int _interactableGridSize;
    [SerializeField, SerializedDictionary("Object Type", "Weight")]
    private SerializedDictionary<GridObjectType, float> _allowedGridObjectTypes;
    [SerializeField, SerializedDictionary("Cube Type", "Weight")]
    private SerializedDictionary<GridCubeType, float> _allowedCubeTypes;
    
    [SerializeField, SerializedDictionary("Defence Item Type", "Count")]
    public SerializedDictionary<GridDefenceItemType, int> _allowedDefenceItemTypes;
    [SerializeField, SerializedDictionary("Enemy Type", "Count")]
    public SerializedDictionary<GridEnemyType, int> _allowedEnemyTypes;
    
    [SerializeField] private byte _moveCount;
    [SerializeField, HideInInspector] private List<GridObjectTypeContainer> _startObjectTypes;
    [SerializeField, HideInInspector] private float _totalGridObjectWeight;
    [SerializeField, HideInInspector] private float _totalGridCubeWeight;
    
    public Vector2Int GridSize => _gridSize;
    public Vector2Int InteractableGridSize => _interactableGridSize;
    public byte MoveCount => _moveCount;
    public List<GridObjectTypeContainer> StartObjectTypes => _startObjectTypes;
    public SerializedDictionary<GridDefenceItemType, int> AllowedDefenceItemTypes=>_allowedDefenceItemTypes;
    // public SerializedDictionary<GridEnemyType, int> AllowedEnemyTypes=>_allowedEnemyTypes;

    private void OnValidate()
    {
        if (_startObjectTypes == null)
            _startObjectTypes = new List<GridObjectTypeContainer>();

        for (int i = 0; i < _gridSize.x * _gridSize.y; i++)
        {
            if (i >= _startObjectTypes.Count)
            {
                _startObjectTypes.Add(new GridObjectTypeContainer());
            }
            else
            {
                if (!_startObjectTypes[i].IsCube && _startObjectTypes[i].GridCubeType != GridCubeType.Invalid)
                {
                    _startObjectTypes[i] = new GridObjectTypeContainer(_startObjectTypes[i].GridObjectType, GridCubeType.Invalid, GridDefenceItemType.Invalid, GridEnemyType.Invalid);
                }
            }
        }

        if (_startObjectTypes.Count > _gridSize.x * _gridSize.y)
        {
            _startObjectTypes.RemoveRange(_gridSize.x * _gridSize.y, _startObjectTypes.Count - _gridSize.x * _gridSize.y);
        }

        _totalGridObjectWeight = 0;
        _totalGridCubeWeight = 0;

        foreach ((_, float value) in _allowedGridObjectTypes)
        {
            _totalGridObjectWeight += value;
        }

        foreach ((_, float value) in _allowedCubeTypes)
        {
            _totalGridCubeWeight += value;
        }
    }
    public void UseMove()
    {
        if (_moveCount == 0) return;
        
        _moveCount--;
        MoveUsed?.Invoke();
    }

    
    public void Randomize()
    {
        _startObjectTypes.Clear();

        for (int y = 0; y < _gridSize.x * _gridSize.y; y++)
        {
            _startObjectTypes.Add(GetRandomObjectType());
        }
    }

    public GridObjectTypeContainer GetRandomObjectType()
    {
        GridObjectTypeContainer startObjectType = new GridObjectTypeContainer();

        float gridObjectValue = Random.Range(0, _totalGridObjectWeight);
        float currentValue = 0;

        foreach ((GridObjectType key, float value) in _allowedGridObjectTypes)
        {
            currentValue += value;

            if (gridObjectValue <= currentValue)
            {
                startObjectType.GridObjectType = key;
                break;
            }
        }

        if (startObjectType.GridObjectType == GridObjectType.Cube)
        {
            float cubeTypeValue = Random.Range(0, _totalGridCubeWeight);
            currentValue = 0;

            foreach ((GridCubeType key, float value) in _allowedCubeTypes)
            {
                currentValue += value;

                if (cubeTypeValue <= currentValue)
                {
                    startObjectType.GridCubeType = key;
                    break;
                }
            }
        }
        return startObjectType;
    }
}

