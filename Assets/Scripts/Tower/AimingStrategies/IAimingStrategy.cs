using UnityEngine;

public struct AimingResult
{
    public Enemy enemy;
    public Vector3 targetPosition;
    public float lookingAngle;
    public bool shouldFire; // this is for in case enemy become null
}

public interface IAimingStrategy
{
    AimingResult UpdateAiming(Vector3 towerPosition, float range);
}
