using UnityEngine;

[CreateAssetMenu(menuName = "Bloodrush/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public string projectileName = "DefaultProjectile"; // name used to get sprites
    public float baseProjectileSpeed = 15;
    public float baseProjectileDamage = 3f;
    public ProjectileMovementType projectileMovementType = ProjectileMovementType.DIRECTED;
    public float baseProjectileColliderRadius = 0.2f;
    public int baseProjectilePierce = 1;
    public Vector3 spriteTransformSize = Vector3.one; // (for making it visually larger or smaller)
    public float baseProjectileAOERadiusOnHit = 0f; // if > 0 then this projectile does AOE damage on hit, and this is the radius of that AOE damage, does AOE damage equal to 'baseProjectileDamage' and need to make sure we don't double hit the same enemy (one originally hit) so we need to circle overlap to get enemies opposed to just getting struck one.
    // public List<ProjectileEffect> projectileEffectsOnHit; // for applying effects like burn, slow, etc. on hit, would be a list of effects to apply on hit, and each effect would have its own stats like duration and strength and such.

}
