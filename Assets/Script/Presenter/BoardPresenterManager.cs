using System.Collections.Generic;
using UnityEngine;

public class BoardPresenterManager
{
    private readonly BoardController _boardController;
    private readonly Dictionary<GridObjectInstanceId, BaseGridObjectPresenter> _presenters =
        new Dictionary<GridObjectInstanceId, BaseGridObjectPresenter>();

    public BoardPresenterManager(BoardController boardController)
    {
        _boardController = boardController;
        RegisterListeners();
    }

    public void Dispose()
    {
        UnregisterListeners();

        foreach (BaseGridObjectPresenter presenter in _presenters.Values)
        {
            presenter.Dispose();
        }
    }

    public void Update(float deltaTime)
    {
    }

    private void RegisterListeners()
    {
        _boardController.GridObjectCreated += OnGridObjectCreated;
        _boardController.GridObjectInteracted += OnGridObjectInteracted;
        _boardController.GridObjectFalling += OnGridObjectFalling;
        _boardController.GridObjectDestroyed += OnGridObjectDestroyed;
    }

    private void UnregisterListeners()
    {
        _boardController.GridObjectCreated -= OnGridObjectCreated;
        _boardController.GridObjectInteracted -= OnGridObjectInteracted;
        _boardController.GridObjectFalling -= OnGridObjectFalling;
        _boardController.GridObjectDestroyed -= OnGridObjectDestroyed;
    }

    private void OnGridObjectFalling(GridObjectInstanceId instanceId, Vector2Int position)
    {
        if (_presenters.TryGetValue(instanceId, out BaseGridObjectPresenter presenter))
        {
            presenter.Fall(position);
        }
    }

    private void OnGridObjectInteracted(GridObjectInstanceId instanceId, bool result)
    {
        if (_presenters.TryGetValue(instanceId, out BaseGridObjectPresenter presenter))
        {
            presenter.OnInteracted(result);
        }
    }

    private void OnGridObjectCreated(GridObjectInstanceId instanceId, GridObjectPresenterData presenterData)
    {
        ResourcesController.Instance.TryGetGridObject(presenterData.GridObjectTypeContainer, out BaseGridObjectPresenter presenter);
        presenter.Initialize(presenterData);
        _presenters.Add(instanceId, presenter);
    }
    
    private void OnGridObjectDestroyed(GridObjectInstanceId instanceId)
    {
        if (_presenters.TryGetValue(instanceId, out BaseGridObjectPresenter presenter))
        {
            presenter.Dispose();
            _presenters.Remove(instanceId);
        }
    }
}