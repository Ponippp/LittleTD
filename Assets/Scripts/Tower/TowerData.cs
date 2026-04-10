using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Bloodrush/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Base Tower Info")]
    public string towerName;
    public TowerType towerType;
    // public string towerDescription; // for when hitting (i) button in-game. Should be appropriately vague.
    // public int towerCost = 650;
    // public TowerFootprint towerFootprint; // POSSIBLE NEW STAT, default to 1x1 but could have other options like 2x2 or 1x2, etc.

    // [Header("Tower Paths")]
    // TODO either have it be a list of tower upgrade paths of "TowerData" or some offshoot of "TowerData" or something else entirely like maybe 
    //      we have base tower data but then all subsequent upgrades are "TowerUpgradeData" as they wont have fields like "baseProjectileSpeed" hmm or 
    //      we could make it where we update the BaseBoostedFloats.baseF during runtime and just have an 'ApplyNewData' method. But on our end does it 
    //      make more sense to have upgrade B ADDING X to field Y or for upgrade B to REPLACE X with Y. Replacement seems cleaner honestly. And makes 
    //      composite pattern easier as well. Can create an editor which based on naming conventions, gets stats from other tiers of a given tower
    //      and has a difference checker and display between tiers so you can see what previous and next tier are and see "ok this is +150% last tier"

    [Header("Base Tower Stats")]
    public float baseRange = 2f;
    public float baseFireInterval = 0.2f;
    // public float baseBulletSpreadAngle = 0f; // POSSIBLE NEW STAT
    // public int projectilesFiredWithEachShot = 1; // POSSIBLE NEW STAT (allows for burst fire or shotguns (as each pellet gets own bullet spread angle))
    //      public float baseReleaseTimeBetweenEachProjectileInBurst = 0.05f; // POSSIBLE NEW STAT (for burst fire towers, how long between each projectile is released after the initial fire command)

    [Header("Aiming Stats")]
    public TowerAimingType towerAimingType = TowerAimingType.FIRST;
    public float baseRotationSpeedIfSpinningRadians = 0;
    // add mechanic for max tower rotation speed where instead of being able to go from facing right to left instantly, it must spin all the way 
    //      around over the course of X time. This could be interesting and would add more weight to faster enemies as well as perhaps incentivize 
    //      player to play towers far away from enemies as then that shortens the rotation time due to the movement within their sight being relatively 
    //      smaller. Ooh I like that. And we can just rename "baseRotationSpeedIfSpinningRadians" to "baseMaxRotationSpeedRadians". TODO

    [Header("Projectile Stats")] // TODO perhaps create a new "ProjectileData" scriptable object for this instead of having it all in TowerData, as then we can have more modularity and reuse of projectiles between towers, as well as easier creation of new projectiles without having to create a whole new tower. 
    // public ProjectileData projectileData;
    public float baseProjectileSpeed = 30f;
    public float baseProjectileDamage = 4f;
    public ProjectileMovementType projectileMovementType = ProjectileMovementType.DIRECTED;
    // public float baseProjectileColliderRadius = 0.25f;
    // public int baseProjectilePierce = 1;
    // public Sprite baseProjectileSprite OR public string baseProjectileSpriteName (and load it in the factory)
    // public Vector3 baseProjectileTransformSize = Vector3.one; // (for making it visually larger or smaller)
    // public float baseProjectileAOERadiusOnHit = 0f; // if > 0 then this projectile does AOE damage on hit, and this is the radius of that AOE damage, does AOE damage equal to 'baseProjectileDamage' and need to make sure we don't double hit the same enemy (one originally hit) so we need to circle overlap to get enemies opposed to just getting struck one.
    // public List<ProjectileEffect> projectileEffectsOnHit; // for applying effects like burn, slow, etc. on hit, would be a list of effects to apply on hit, and each effect would have its own stats like duration and strength and such.

    [Header("Visual Stats")]
    public float fireAnimationTime = 0.1f;
    public Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
    public float projectileSpawnRingRadius = 0.75f;
    // public string animationeNamePrefixForSpriteLoading = ""; 
}
