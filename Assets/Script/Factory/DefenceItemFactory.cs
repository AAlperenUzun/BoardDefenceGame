public class DefenceItemFactory : IObjectFactory
{
    public BaseGridObjectController Create(GridObjectData data, GridObjectInstanceId instanceId)
    {
        return new DefenceItemController(data, instanceId);
    }
}