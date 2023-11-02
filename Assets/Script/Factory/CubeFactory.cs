public class CubeFactory : IObjectFactory
{
    public BaseGridObjectController Create(GridObjectData data, GridObjectInstanceId instanceId)
    {
        return new CubeObjectController(data, instanceId);
    }
}