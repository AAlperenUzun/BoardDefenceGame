public class EnemyFactory : IObjectFactory
{
    public BaseGridObjectController Create(GridObjectData data, GridObjectInstanceId instanceId)
    {
        return new EnemyObjectController(data, instanceId);
    }
}