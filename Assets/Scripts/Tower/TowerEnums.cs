
public enum TowerState
{
    IDLE,
    SHOOT
}

public enum ProjectileMovementType
{
    DIRECTED,
    HOMING
}

public enum TowerType
{
    GIGA_GATLING,
    JUICY_RAG_LAUNCHER,
}

public enum TowerAimingType
{
    FIRST,
    CLOSEST,
    STRONGEST,
    LAST,
    WEAKEST,
    SPIN, // doesnt aim, just spins and fires in any direction
    // add more aiming types here like: random, etc
}