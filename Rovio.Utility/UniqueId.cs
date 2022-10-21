namespace Rovio.Utility;

public struct UniqueId
{
    public static implicit operator uint(UniqueId uniqueId) => uniqueId._value;
    public static implicit operator UniqueId(int id) => (uint)id;
    public static implicit operator UniqueId(uint id) => new(id);

    private uint _value;
    
    public UniqueId(uint id)
    {
        _value = id;
    }

    public bool IsInvalid() => _value == 0;
    
    public override string ToString()
    {
        return _value.ToString();
    }

    public override bool Equals(object? other)
    {
        return Equals((UniqueId)(other ?? 0));
    }

    public bool Equals(UniqueId other)
    {
        return _value == other._value;
    }
}