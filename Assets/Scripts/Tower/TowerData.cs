using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "TD/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Base Tower Stats")]
    public float baseRange = 2f;
    public float baseFireInterval = 0.2f;

    [Header("Projectile Stats")]
    public float baseProjectileSpeed = 30f;
    public float baseProjectileDamage = 4f;
    public TowerProjectileAimingType aimingType = TowerProjectileAimingType.DIRECTED;

    [Header("Visual Stats")]
    public float fireAnimationTime = 0.1f;
    public Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
    public float projectileSpawnRingRadius = 0.75f;
    // put animations here
    // put tower uprgade tree here with composite pattern
}
