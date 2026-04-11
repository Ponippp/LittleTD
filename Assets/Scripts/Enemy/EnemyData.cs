using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Bloodrush/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "DefaultName";
    public EnemyType enemyType;

    [Header("Stats")]
    public float health = 50f;
    public float speed = 1f;
    public float circleColliderRadius = 0.3f;
    public int animationSpeedPercentage = 100;
    public EnemyMovementType movementType = EnemyMovementType.GROUND;
}
