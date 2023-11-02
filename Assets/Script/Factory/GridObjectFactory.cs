using System;
using System.Collections.Generic;

public class GridObjectFactory
{
    private readonly Dictionary<GridObjectType, IObjectFactory> _factories = new Dictionary<GridObjectType, IObjectFactory>()
    {
        { GridObjectType.Cube, new CubeFactory() },
        // { GridObjectType.Balloon, new BaloonFactory() },
        // { GridObjectType.Rocket, new RocketFactory() },
        { GridObjectType.DefenceItem, new DefenceItemFactory() },
        { GridObjectType.Enemy, new EnemyFactory() },
    };
    
    private bool TryGetCubeFactory(GridObjectType type, out IObjectFactory factory)
    {
        return _factories.TryGetValue(type, out factory);
    }
    
    public BaseGridObjectController Create(GridObjectData data, GridObjectInstanceIdProvider instanceIdProvider)
    {
        if (TryGetCubeFactory(data.TypeContainer.GridObjectType, out IObjectFactory factory))
        {
            return factory.Create(data, instanceIdProvider.CreateInstanceId());
        }

        throw new Exception($"There is no factory with {data.TypeContainer.GridObjectType}");
    }
}