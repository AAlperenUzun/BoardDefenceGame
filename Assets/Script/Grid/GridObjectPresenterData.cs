using UnityEngine;

public readonly struct GridObjectPresenterData
{
    public readonly GridObjectInstanceId InstanceId;
    public readonly Vector2Int Position;
    public readonly GridObjectTypeContainer GridObjectTypeContainer;
    public readonly GridObjectState ObjectState;
    public readonly Axis Axis;

    public static readonly GridObjectPresenterData Invalid =
        new GridObjectPresenterData(GridObjectInstanceId.Invalid, Vector2Int.zero, GridObjectTypeContainer.Invalid, GridObjectState.Invalid, Axis.None);
    
    public bool IsValid => InstanceId.IsValid;

    public GridObjectPresenterData(GridObjectInstanceId instanceId, Vector2Int position, GridObjectTypeContainer gridObjectTypeContainer, GridObjectState objectState, Axis axis)
    {
        InstanceId = instanceId;
        Position = position;
        GridObjectTypeContainer = gridObjectTypeContainer;
        ObjectState = objectState;
        Axis = axis;
    }
}