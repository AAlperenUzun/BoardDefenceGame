using UnityEngine;

public static class GameUtility
{
    public const int DirectionCount = 4;
    public const int MinMatchCountForRocket = 5;
    public const float FallSpeed = 12;
    public const float RocketSpeed = 10;

    private static readonly int[] _rowDirections = { -1, 0, 0, 1 };
    private static readonly int[] _colDirections = { 0, -1, 1, 0 };
    
    public static float FallDuration => 1f / FallSpeed;
    public static float RocketDuration => 1f / RocketSpeed;

    public static int GetRowDirection(int index) => _rowDirections[index];
    public static int GetColDirection(int index) => _colDirections[index];

    public static bool IsMatching(GridObjectPresenterData source, GridObjectPresenterData target)
    {
        if (source.ObjectState != GridObjectState.Idle || target.ObjectState != GridObjectState.Idle)
            return false;
        
        if (source.GridObjectTypeContainer.GridObjectType == GridObjectType.DefenceItem)
            return false;

        if (target.GridObjectTypeContainer.GridObjectType == GridObjectType.DefenceItem)
            return true;

        if (source.GridObjectTypeContainer.GridObjectType == GridObjectType.Cube &&
            source.GridObjectTypeContainer.GridCubeType == target.GridObjectTypeContainer.GridCubeType)
            return true;

        return false;
    }

    public static Axis GetRandomAxisForRocket()
    {
        if (Random.value < 0.5f)
            return Axis.Horizontal;
        
        return Axis.Vertical;
    }
}