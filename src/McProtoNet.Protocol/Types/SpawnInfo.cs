namespace McProtoNet.Protocol;

public sealed class SpawnInfo
{
    public int Dimension { get;}
    public string Name { get;  }
    public long HashedSeed { get;}
    public byte Gamemode { get;  }
    public byte PreviousGamemode { get; }
    public bool IsDebug { get;  }
    public bool IsFlat { get;}
    public DeathLocation Death { get;  }
    public int PortalCooldown { get; }
    public int? SeaLevel { get; }

    public SpawnInfo(int dimension, string name, long hashedSeed, byte gamemode, byte previousGamemode, bool isDebug, bool isFlat, DeathLocation death, int portalCooldown, int? seaLevel)
    {
        Dimension = dimension;
        Name = name;
        HashedSeed = hashedSeed;
        Gamemode = gamemode;
        PreviousGamemode = previousGamemode;
        IsDebug = isDebug;
        IsFlat = isFlat;
        Death = death;
        PortalCooldown = portalCooldown;
        SeaLevel = seaLevel;
    }
}