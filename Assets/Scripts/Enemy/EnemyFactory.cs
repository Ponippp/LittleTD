using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    [Header("Enemy Data (Defaults)")]
    [SerializeField] private EnemyData cromenockleData;
    [SerializeField] private EnemyData miteData;
    [SerializeField] private EnemyData blartData;
    [SerializeField] private EnemyData peterData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.GetIsInitialized())
            {
                EnemyData data = GetDataByType(enemy.GetEnemyType());
                Vector3 spawn = GameManager.instance != null ? GameManager.instance.GetEnemySpawnPoint() : enemy.transform.position;
                enemy.Initialize(CreateEnemyStats(data), spawn);
            }
        }
    }

    public Enemy CreateEnemy(EnemyType type, Vector3 spawnPosition)
    {
        GameObject prefabGo = GameManager.instance.GetEnemyPrefab();
        EnemyData data = GetDataByType(type);

        if (prefabGo == null || data == null)
        {
            Debug.LogError($"[EnemyFactory] Missing prefab or data for {type}");
            return null;
        }

        Enemy enemy = Instantiate(prefabGo, spawnPosition, Quaternion.identity).GetComponent<Enemy>(); // TODO change to pooling

        enemy.Initialize(CreateEnemyStats(data), spawnPosition);
        return enemy;
    }

    public Enemy.EnemyStats CreateEnemyStats(EnemyData data)
    {
        return new Enemy.EnemyStats
        {
            enemyName = data.enemyName,
            health = data.health,
            speed = data.speed,
            circleColliderRadius = data.circleColliderRadius,
            animationSpeedPercentage = data.animationSpeedPercentage,
            pathfinding = new Enemy.EnemyStats.Pathfinding
            {
                movementType = data.movementType,
            },
            record = new Enemy.EnemyStats.Record
            {
                enemyType = data.enemyType,
                isInitialized = false,
            },
        };
    }

    public EnemyData GetDataByType(EnemyType type)
    {
        return type switch
        {
            EnemyType.CROMENOCKLE => cromenockleData,
            EnemyType.MITE => miteData,
            EnemyType.BLART => blartData,
            EnemyType.PETER => peterData,
            _ => null,
        };
    }
}
