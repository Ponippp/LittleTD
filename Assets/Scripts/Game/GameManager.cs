using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

//TODO:
/*
Use facade pattern to run a complex game. Instead of loadMap(), loadEnemies(), play(), endGame(),
we will just have a method runGame() that does all of these things
*/

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [Header("Setup Floor Grid")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private Vector3 gridOffset = Vector3.zero;
    [Header("Game State")]
    [SerializeField] private Vector3 enemySpawnPoint;
    [SerializeField] private Vector3 enemyGoalPoint;
    [Header("Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private AnimatorOverrideController overrideController;

    /// <summary>Template duplicated per enemy; assign EnemyAnimatorOverrideController in the inspector.</summary>
    public static AnimatorOverrideController EnemyAnimatorOverrideTemplate => instance != null ? instance.overrideController : null;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
        Utility.InitializeLayerMasks();
        SetupObjectPools();
    }

    private void SetupObjectPools()
    {
        ObjectPooler.SetupPool(projectilePrefab.GetComponent<Projectile>(), 100, Utility.PROJECTILE_OBJECTPOOL_NAME);
    }

    /// <summary>
    /// Is IEnumerator return type as to be able to stall a frame when setting up the A* grid.
    /// </summary>
    private IEnumerator Start() 
    {
        EventsManager.instance.gameEvents.SetupNewAStarGrid(gridHeight, gridWidth, gridOffset, floorTilemap);
        yield return null; // Wait one frame to ensure all objects (Enemies, Towers) have initialized and subscribed to events
        EventsManager.instance.gameEvents.TowerGridUpdated();
    }

    public Vector3 GetEnemySpawnPoint() => enemySpawnPoint;
    public Vector3 GetEnemyGoalPoint() => enemyGoalPoint;
    public Tilemap GetFloorTilemap() => floorTilemap;
    public GameObject GetTowerPrefab() => towerPrefab;
    public GameObject GetEnemyPrefab() => enemyPrefab;
    // public GameObject GetProjectilePrefab() => projectilePrefab;
    public AnimatorOverrideController GetAnimatorOverrideController() => overrideController;
    public int GetGridHeight() => gridHeight;
    public int GetGridWidth() => gridWidth;
    public Vector3 GetGridOffset() => gridOffset;
}