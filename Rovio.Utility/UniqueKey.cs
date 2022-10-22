using System.Text.Json.Serialization;

namespace Rovio.Utility;

public struct UniqueKey
{
    public static implicit operator uint(UniqueKey uniqueKey) => uniqueKey._value;
    public static implicit operator UniqueKey(int id) => (uint)id;
    public static implicit operator UniqueKey(uint id) => new(id);

    [JsonPropertyName("v")]
    public uint Value
    {
        get => _value;
        init => _value = value;
    }
    
    private uint _value;
    
    public UniqueKey(uint id)
    {
        _value = id;
    }

    public bool IsValid() => _value > 0;
    
    public override string ToString()
    {
        return _value.ToString();
    }

    public override bool Equals(object? other)
    {
        return Equals((UniqueKey)(other ?? 0));
    }

    public bool Equals(UniqueKey other)
    {
        return _value == other._value;
    }
}