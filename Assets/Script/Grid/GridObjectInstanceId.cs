using System;

public readonly struct GridObjectInstanceId : IEquatable<GridObjectInstanceId>
{
    public readonly ushort Value;

    public bool IsValid => Value != Invalid.Value;

    public GridObjectInstanceId(ushort value)
    {
        Value = value;
    }

    public bool Equals(GridObjectInstanceId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        return obj is GridObjectInstanceId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    
    public static readonly GridObjectInstanceId Invalid = new GridObjectInstanceId(0);
}