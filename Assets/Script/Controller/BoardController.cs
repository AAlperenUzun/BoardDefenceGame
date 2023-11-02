using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController
{
    public event Action<GridObjectInstanceId, GridObjectPresenterData> GridObjectCreated;
    public event Action<GridObjectInstanceId, bool> GridObjectInteracted;
    public event Action<GridObjectInstanceId> GridObjectDestroyed;
    public event Action<GridObjectInstanceId, Vector2Int> GridObjectFalling;

    private readonly InputController _inputController;
    private readonly GridObjectInstanceIdProvider _instanceIdProvider;
    private readonly GridObjectFactory _factory;
    private readonly LevelData _levelData;
    private List<GridEnemyType> _waitingEnemies=new List<GridEnemyType>();

    [NonSerialized] public List<InventoryItem> InventoryItems = new List<InventoryItem>();
    // private List<GridDefenceItemType> _waitingDefenceItems=new List<GridDefenceItemType>();
    [NonSerialized]
    public List<EnemyObjectController> _createdEnemies=new List<EnemyObjectController>();
    
    public List<DefenceItemController> _createdDefenceItems=new List<DefenceItemController>();
    public Grid Grid { get; private set; }
    public bool IsLocked { get; private set; }
    public float blockTime=3f;

    public BoardController(InputController inputController, LevelData levelData)
    {
        _inputController = inputController;
        _instanceIdProvider = new GridObjectInstanceIdProvider();
        _factory = new GridObjectFactory();
        _levelData = levelData;
        var x = 0;
        foreach (var enemytypes in _levelData._allowedEnemyTypes)
        {
            for (int i = 0; i < enemytypes.Value; i++)
            {
                _waitingEnemies.Add(enemytypes.Key);
            }
        }

        Initialize();
    }

    public void Initialize()
    {
        RegisterListeners();
    }

    public void Dispose()
    {
        UnregisterListeners();
    }

    public void Update(float deltaTime)
    {
        Grid.Update(deltaTime);
        foreach (var enemy in _createdEnemies)
        {
            enemy.currentBlockTime+= enemy._enemy.speed* deltaTime;
            if (blockTime<=enemy.currentBlockTime)
            {
                enemy.currentBlockTime = 0;
                enemy.StartFall();
            }
        }

        foreach (var defenceItem in _createdDefenceItems)
        {
            defenceItem.CurrentIntervalTime += defenceItem.DefenceItem.interval * deltaTime;
            if (blockTime<=defenceItem.CurrentIntervalTime)
            {
                defenceItem.CurrentIntervalTime = 0;
                defenceItem.Attack();
            }
        }
    }

    public void GenerateEnemies()
    {
        var tempGrid = new Grid((byte)_levelData.GridSize.y, (byte)_levelData.GridSize.x);
        
        for (var i = Grid.GridObjects.Length-Grid.GridObjects.Length/_levelData.GridSize.y; i < tempGrid.GridObjects.Length; i++)
        {
            Vector2Int position = tempGrid.ToTwoDimensionIndex(i);

            if (_waitingEnemies.Count <= 0)
            {
                break;
            }
            GridObjectData data = new GridObjectData(position, GridObjectType.Enemy, GridCubeType.Invalid, GridDefenceItemType.Invalid, GetRandomEnemy(), GridObjectState.Idle,
                Axis.None);
            
            TryAddEnemy(position, data);
        }
    }
    
    public void Generate()
    {
        Grid = new Grid((byte)_levelData.GridSize.y, (byte)_levelData.GridSize.x);

        for (var i = 0; i < Grid.GridObjects.Length; i++)
        {
            Vector2Int position = Grid.ToTwoDimensionIndex(i);
            GridObjectTypeContainer typeContainer;

            if (_levelData.StartObjectTypes.Count > i)
            {
                typeContainer = _levelData.StartObjectTypes[i];
            }
            else
            {
                typeContainer = _levelData.GetRandomObjectType();
            }

            GridObjectData data = new GridObjectData(position, typeContainer.GridObjectType, typeContainer.GridCubeType, typeContainer.GridDefenceItemType, typeContainer.GridEnemyType, GridObjectState.Idle,
                Axis.None);
            CreateGridObject(data);
        }
    }
    private void CreateGridEnemy(GridObjectData objectData)
    {
        BaseGridObjectController controller = _factory.Create(objectData, _instanceIdProvider);
        AddGridEnemy(controller);
    }
    private void AddGridEnemy(BaseGridObjectController controller)
    {
        Grid.SetGridObject(controller);
    }
    private void CreateGridObject(GridObjectData objectData)
    {
        BaseGridObjectController controller = _factory.Create(objectData, _instanceIdProvider);
        AddGridObject(controller);
    }

    private void AddGridObject(BaseGridObjectController controller)
    {
        Grid.SetGridObject(controller);
        GridObjectCreated?.Invoke(controller.InstanceId, controller.GetPresenterData());
    }

    private GridEnemyType GetRandomEnemy()
    {
        var x = Random.Range(0, _waitingEnemies.Count);
        var value = _waitingEnemies[x];
        _waitingEnemies.RemoveAt(x);
        return value;
    }

    public bool TryAddEnemy(Vector2Int position, GridObjectData objectData)
    {
        GridObjectTypeContainer startObjectType = new GridObjectTypeContainer();
        startObjectType.GridObjectType = GridObjectType.Enemy;
        startObjectType.GridEnemyType = objectData.TypeContainer.GridEnemyType;
        var typeContainer = startObjectType;
        objectData = new GridObjectData(position, typeContainer.GridObjectType, GridCubeType.Invalid, GridDefenceItemType.Invalid, typeContainer.GridEnemyType, GridObjectState.Invalid,
            Axis.None);
        if (!Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller))
        {
            return false;
        }

        Grid.SetGridObject(null, position.x, position.y);

        GridObjectDestroyed?.Invoke(controller.InstanceId);
        var enemyC = (EnemyObjectController)_factory.Create(objectData, _instanceIdProvider);
        _createdEnemies.Add(enemyC);
        enemyC.Initialize();
        AddGridObject(enemyC);

        return true;
    }
    public bool TryAddDefenceItem(Vector2Int position, GridObjectData objectData)
    {
        if (InventoryItems.Count <= 0) return false;
        GridObjectTypeContainer startObjectType = new GridObjectTypeContainer();
        startObjectType.GridObjectType = GridObjectType.DefenceItem;
        var createdI = InventoryItems[0];
        startObjectType.GridDefenceItemType = createdI._defenceItemType;
        var typeContainer = startObjectType;
        // var typeContainer = GameController.Instance.BoardController._levelData.GetRandomObjectType();
        objectData = new GridObjectData(position, GridObjectType.DefenceItem, GridCubeType.Invalid, typeContainer.GridDefenceItemType, GridEnemyType.Invalid, GridObjectState.Idle,
            Axis.None);
        if (!Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller))
        {
            return false;
        }

        Grid.SetGridObject(null, position.x, position.y);

        GridObjectDestroyed?.Invoke(controller.InstanceId);

        var defenceI = (DefenceItemController)_factory.Create(objectData, _instanceIdProvider);
        defenceI.Initialize();
        InventoryItems.RemoveAt(0);
        createdI.DestroySelf();
        AddGridObject(defenceI);
        _createdDefenceItems.Add(defenceI);

        return true;
    }
    public bool TryDestroyGridObject(Vector2Int position, bool updateAndFall = true)
    {
        if (!Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller))
        {
            return false;
        }

        Grid.SetGridObject(null, position.x, position.y);
        GridObjectDestroyed?.Invoke(controller.InstanceId);

        if (updateAndFall)
            UpdateAndFall(new List<Vector2Int>() { position }, GridEnemyType.Invalid);

        return true;
    }
    public bool AttackToEnemy(Vector2Int position, float damage)
    {
        if (!Grid.TryGetGridObject(position.x, position.y, out BaseGridObjectController controller))
        {
            return false;
        }
        if (controller.Data.TypeContainer.GridObjectType!=GridObjectType.Enemy)
        {
            return false;
        }
        var enemyC = (EnemyObjectController)controller;
        var isDead=enemyC.TakeDamage(damage);
        if (isDead)
        {
            _createdEnemies.Remove((EnemyObjectController)controller);
            Grid.SetGridObject(null, position.x, position.y);
            GridObjectDestroyed?.Invoke(controller.InstanceId);
            
         
            if (_waitingEnemies.Count>0)
            {
                GridObjectData data = new GridObjectData(position, GridObjectType.Enemy, GridCubeType.Invalid, GridDefenceItemType.Invalid, GetRandomEnemy(), GridObjectState.Idle,
                    Axis.None);
                UpdateAndFall(new List<Vector2Int>() { position}, data.TypeContainer.GridEnemyType);
            }
            else
            {
                UpdateAndFall(new List<Vector2Int>() { position},GridEnemyType.Invalid);
            }
            if (_createdEnemies.Count<=0 && _waitingEnemies.Count<=0)
            {
                GameController.Instance.EndLevel(true);
            }
        }
        return true;
    }

    private IEnumerator<float> CreateAndFallRoutine(List<Vector2Int> positions, GridEnemyType enemyType)
    {
        int lastY = positions[0].y;
        Dictionary<int, int> yOffsetsByXPositions = new Dictionary<int, int>();

        foreach (Vector2Int position in positions)
        {
            if (position.y != lastY)
            {
                lastY = position.y;
                yield return Timing.WaitForSeconds(.05f);
            }

            int y = Grid.GetAvailableEmptyYPosition(position.x);
            Vector2Int newPosition = new Vector2Int(position.x, y);
            Vector2Int startPosition = newPosition;

            if (!yOffsetsByXPositions.TryGetValue(position.x, out int yOffset))
            {
                yOffset = 5;
            }

            startPosition.y = Grid.TopY + yOffset;
            yOffsetsByXPositions[position.x] = yOffset + 1;

            GridObjectTypeContainer typeContainer = _levelData.GetRandomObjectType();
            if (enemyType!=GridEnemyType.Invalid)
            {
                typeContainer.GridObjectType = GridObjectType.Enemy;
                typeContainer.GridEnemyType = enemyType;
            }

            GridObjectData data = new GridObjectData(startPosition, typeContainer.GridObjectType, typeContainer.GridCubeType, typeContainer.GridDefenceItemType, typeContainer.GridEnemyType, GridObjectState.Idle,
                Axis.None);

            BaseGridObjectController cubeController = _factory.Create(data, _instanceIdProvider);
            GridObjectCreated?.Invoke(cubeController.InstanceId, cubeController.GetPresenterData());

            Grid.SetGridObject(cubeController, newPosition.x, newPosition.y);
            cubeController.StartFalling(newPosition);
            if (enemyType != GridEnemyType.Invalid)
            {
                var enemyController=((EnemyObjectController)cubeController);
                enemyController.Initialize();
                _createdEnemies.Add(enemyController);
            }

            GridObjectFalling?.Invoke(cubeController.InstanceId, newPosition);
        }
    }

    public void UpdateAndFall(List<Vector2Int> positions, GridEnemyType enemyType)
    {
        Grid.UpdatePositionsToFillSpaces();

        foreach ((BaseGridObjectController controller, Vector2Int position) in Grid.GetUpdatedPositions())
        {
            controller.StartFalling(position);
            GridObjectFalling?.Invoke(controller.InstanceId, position);
        }

        Timing.RunCoroutine(CreateAndFallRoutine(positions, enemyType));
    }

    private void RegisterListeners()
    {
        _inputController.Clicked += OnClicked;
    }

    private void UnregisterListeners()
    {
        _inputController.Clicked -= OnClicked;
    }

    private void OnClicked(Vector3 point)
    {
        if (IsLocked) return;
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));

        if (Grid.TryGetGridObject(gridPosition.x, gridPosition.y, out BaseGridObjectController controller))
        {
            bool successful = controller.Interact();
            GridObjectInteracted?.Invoke(controller.InstanceId, successful);
            
            if (successful)
            {
                _levelData.UseMove();
            }
        }
    }
}