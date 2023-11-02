using System;
using AYellowpaper.SerializedCollections;
using Lean.Pool;
using UnityEngine;

public class ResourcesController : SingletonBehaviour<ResourcesController>
{
    [Serializable]
    private class GridObjectsByType : SerializedDictionary<GridObjectType, BaseGridObjectPresenter>
    {
    }
    
    [Serializable]
    private class GridCubeObjectsByType : SerializedDictionary<GridCubeType, CubeObjectPresenter>
    {
    }
    [Serializable]
    private class GridDefenceItemsByType : SerializedDictionary<GridDefenceItemType, DefenceItemPresenter>
    {
    }
    [Serializable]
    private class GridEnemiesByType : SerializedDictionary<GridEnemyType, EnemyObjectPresenter>
    {
    }
    [Serializable]
    private class SpritesByTypeContainer : SerializedDictionary<GridObjectTypeContainer, Sprite>
    {
    }
    
    


    [SerializeField] private GridObjectsByType _gridObjectsByType;
    [SerializeField] private GridCubeObjectsByType _cubeObjectsByType;
    [SerializeField] private GridDefenceItemsByType _defenceItemsByType;
    [SerializeField] private GridEnemiesByType _enemiesByType;
    [SerializeField] private SpritesByTypeContainer _spritesByTypeContainer;
    

    protected override void OnAwake()
    {
    }
    
    public bool TryGetGridObject(GridObjectTypeContainer typeContainer, out BaseGridObjectPresenter gridObjectPresenter)
    {
        if (typeContainer.IsCube && TryGetCubeGridObject(typeContainer.GridCubeType, out CubeObjectPresenter cubeObjectPresenter))
        {
            gridObjectPresenter = cubeObjectPresenter;
            return true;
        }
        
        if (typeContainer.GridObjectType==GridObjectType.DefenceItem && TryGetDefenceGridItem(typeContainer.GridDefenceItemType, out DefenceItemPresenter defenceItemPresenter))
        {
            gridObjectPresenter = defenceItemPresenter;
            return true;
        }
        
        if (typeContainer.GridObjectType==GridObjectType.Enemy && TryGetEnemyGridObject(typeContainer.GridEnemyType, out EnemyObjectPresenter enemyPresenter))
        {
            gridObjectPresenter = enemyPresenter;
            return true;
        }
        
        if (_gridObjectsByType.TryGetValue(typeContainer.GridObjectType, out BaseGridObjectPresenter prefab))
        {
            gridObjectPresenter = LeanPool.Spawn(prefab);
            return true;
        }

        gridObjectPresenter = null;
        return false;
    }
    
    public bool TryGetCubeGridObject(GridCubeType cubeType, out CubeObjectPresenter cubeObjectPresenter)
    {
        if (_cubeObjectsByType.TryGetValue(cubeType, out CubeObjectPresenter prefab))
        {
            cubeObjectPresenter = LeanPool.Spawn(prefab);
            return true;
        }

        cubeObjectPresenter = null;
        return false;
    }
    
    public bool TryGetDefenceGridItem(GridDefenceItemType defenceItemType, out DefenceItemPresenter defenceItemPresenter)
    {
        if (_defenceItemsByType.TryGetValue(defenceItemType, out DefenceItemPresenter prefab))
        {
            defenceItemPresenter = LeanPool.Spawn(prefab);
            return true;
        }

        defenceItemPresenter = null;
        return false;
    }
    
    public bool TryGetEnemyGridObject(GridEnemyType defenceItemType, out EnemyObjectPresenter defenceItemPresenter)
    {
        if (_enemiesByType.TryGetValue(defenceItemType, out EnemyObjectPresenter prefab))
        {
            defenceItemPresenter = LeanPool.Spawn(prefab);
            return true;
        }

        defenceItemPresenter = null;
        return false;
    }

    public bool TryGetSprite(GridObjectTypeContainer typeContainer, out Sprite sprite)
    {
        if (_spritesByTypeContainer.TryGetValue(typeContainer, out sprite))
        {
            return true;
        }

        return false;
    }
}