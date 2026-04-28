using UnityEngine;

[CreateAssetMenu(menuName = "Bloodrush/WaveData")]
public class WaveData : ScriptableObject
{
    public float sendInterval = 1f;
    public int enemiesInWave = 20;
    public EnemyType enemyType = EnemyType.MITE;
}