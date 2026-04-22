using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; } //compressed syntax for singleton pattern w/ getter and setter

    [Header("Enemy Data (Defaults)")]
    [SerializeField] private EnemyData cromenockleData;
    [SerializeField] private EnemyData miteData;
    [SerializeField] private EnemyData blartData;
    [SerializeField] private EnemyData peterData;

    private void Awake() //runs when object is created. once an object is created, it is recycled with OnEnable() and OnDisable(), but Awake() is neevr urn again for that object
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() //runs after Awake(), think of it liek a priority queue
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None); 
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.GetIsInitialized())
            {
                EnemyData data = GetDataByType(enemy.GetEnemyType());
                Vector3 spawn = GameManager.instance != null ? GameManager.instance.GetEnemySpawnPoint() : enemy.transform.position; //could do == and swap order of ? operator
                enemy.Initialize(CreateEnemyStats(data), spawn);
            }
        }
    }

    public Enemy CreateEnemy(EnemyType type, Vector3 spawnPosition)
    {
        GameObject prefabGo = GameManager.instance.GetEnemyPrefab(); //enemy prefab is the enemy game object
        EnemyData data = GetDataByType(type);

        if (prefabGo == null || data == null)
        {
            Debug.LogError($"[EnemyFactory] Missing prefab or data for {type}");
            return null;
        }

        //Instantiate() returns a GameObject, so we need to get the Enemy component from it.
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
                movementType = data.movementType, //overwriting movement type from pathfinding class
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
