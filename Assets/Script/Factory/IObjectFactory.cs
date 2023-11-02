public interface IObjectFactory
{
    BaseGridObjectController Create(GridObjectData data, GridObjectInstanceId instanceId);
}