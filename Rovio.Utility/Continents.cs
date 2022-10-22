namespace Rovio.Utility;

[Flags]
public enum Continents
{
    None    = 1 << 0,
    EU      = 1 << 1,
    NA      = 1 << 2,
    SA      = 1 << 3,
    AS      = 1 << 4,
    OC      = 1 << 5,
    AF      = 1 << 6,
}