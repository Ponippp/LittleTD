using UnityEngine;

// UNUSED STUB
[CreateAssetMenu(menuName = "Bloodrush/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "DefaultName"; // name used to get sprites
    public float health = 50f;
    public float speed = 1f;
    public float circleColliderRadius = .3f;
    public int animationSpeedPercentage = 100;
    public EnemyMovementType movementType = EnemyMovementType.GROUND;
}
