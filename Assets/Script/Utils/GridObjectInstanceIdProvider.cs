public class GridObjectInstanceIdProvider
{
    private const ushort InitialUnitId = 1;
        
    private ushort _id;

    public GridObjectInstanceIdProvider()
    {
        _id = InitialUnitId;
    }

    public GridObjectInstanceId CreateInstanceId()
    {
        return new GridObjectInstanceId(_id++);
    }
}