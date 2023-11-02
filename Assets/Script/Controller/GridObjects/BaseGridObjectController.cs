using UnityEngine;

public abstract class BaseGridObjectController
{
    private float _remainingFallDuration;
    
    public GridObjectData Data { get; }
    public GridObjectInstanceId InstanceId { get; }

    public BaseGridObjectController(GridObjectData data, GridObjectInstanceId instanceId)
    {
        Data = data;
        InstanceId = instanceId;
    }

    public void Update(float deltaTime)
    {
        if (_remainingFallDuration > 0)
        {
            _remainingFallDuration -= deltaTime;
            if (_remainingFallDuration <= 0)
            {
                Data.ObjectState = GridObjectState.Idle;
                FallCompleted();
            }
        }
    }

    public abstract bool Interact();

    public Vector2Int GetPosition() => Data.Position;
    
    public virtual void StartFalling(Vector2Int position)
    {
        if (position.y == Data.Position.y)
        {
            FallCompleted();
            return;
        }
        
        int yDif = Data.Position.y - position.y;
        _remainingFallDuration = GameUtility.FallDuration * yDif;
        Data.ObjectState = GridObjectState.Falling;
        Data.Position = position;
    }

    public GridObjectPresenterData GetPresenterData()
    {
        return new GridObjectPresenterData(InstanceId, Data.Position, Data.TypeContainer, Data.ObjectState, Data.Axis);
    }

    protected virtual void FallCompleted()
    {
        
    }
}