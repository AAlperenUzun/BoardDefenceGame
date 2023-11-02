using System.Collections;
using Lean.Pool;
using Unity.Mathematics;
using UnityEngine;

public class GameController : SingletonBehaviour<GameController>
{
    [SerializeField] private LevelData[] _levels;
    [SerializeField] private GridPresenter _gridPresenter;
    [SerializeField] private UIContoller _uiController;

    private LevelData _currentLevelData;
    private bool _checkingLevelEnd;
    private int _levelIndex;
    

    public InputController InputController { get; private set; }
    public BoardController BoardController { get; private set; }
    public BoardPresenterManager BoardPresenterManager { get; private set; }

    public DefenceItemData defenceItemData;
    public EnemyData enemyData;
    public GameObject starEffect;

    protected override void OnAwake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void Update()
    {
        UpdateSystems();
    }

    public void Initialize()
    {
        InitializeEssentials();
    }

    public void Dispose()
    {
        DisposeEssentials();
    }

    public void Reinitialize(bool progressLevel = false)
    {
        if (progressLevel)
        {
            _levelIndex++;
        }

        Dispose();
        Initialize();
    }
    public void PlayAttackEffect(Vector2Int position)
    {
        var tempEffect= LeanPool.Spawn(starEffect, new Vector3(position.x, position.y, 0), quaternion.identity, transform);
        LeanPool.Despawn(tempEffect, 0.5f);
    }
    private LevelData GetLevel()
    {
        if (_levelIndex >= _levels.Length)
        {
            _levelIndex = 0;
        }

        return _levels[_levelIndex];
    }

    private void InitializeEssentials()
    {
        _currentLevelData = Instantiate(GetLevel());

        InputController = new InputController();
        BoardController = new BoardController(InputController, _currentLevelData);
        BoardPresenterManager = new BoardPresenterManager(BoardController);

        BoardController.Generate();
        _gridPresenter.Initialize(_currentLevelData);

        _uiController.Initialize(_currentLevelData);
        BoardController.GenerateEnemies();

    }


    private void DisposeEssentials()
    {
        InputController.Dispose();
        BoardController.Dispose();
        BoardPresenterManager.Dispose();

        _uiController.Dispose();
    }

    public void EndLevel(bool success)
    {
        _uiController.ShowEndScreen(success);
    }

    private void UpdateSystems()
    {
        float deltaTime = Time.deltaTime;

        BoardController.Update(deltaTime);
        BoardPresenterManager.Update(deltaTime);
    }
}