using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Security;

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
    [SerializeField] private bool gameActive = true;
    [Header("Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private AnimatorOverrideController overrideController;

    [Header("Coins")]
    [SerializeField] private int startingCoins = 2000;
    private int currentCoins;

    [Header("Lives")]
    [SerializeField] private int startingLives = 100;
    private int currentLives;

    [Header("Waves")]
    [SerializeField] private int currentWaveIndex = 0;

    /// <summary>Template duplicated per enemy; assign EnemyAnimatorOverrideController in the inspector.</summary>
    public static AnimatorOverrideController EnemyAnimatorOverrideTemplate => instance != null ? instance.overrideController : null;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
        Utility.InitializeLayerMasks();
        SetupObjectPools();

        currentCoins = startingCoins;
        currentLives = startingLives;
    }

    private void SetupObjectPools()
    {
        //porjectile prefab is a game obj. the Projectile component is the Projectile class in projectile.cs.
        //this gets that "script" and passes it in as the first arg. projectile prefab has several components
        //like sprite renderer, collider, transform, etc.

        //2nd arg is starting number of projectiles. we can never have fewer than 100, but we have no upper limit
        //3rd arg is the name of the object pool we want to put these projectiles in. we have one pool for each type of projectile, so we specify which one with the name.
        ObjectPooler.SetupPool(projectilePrefab.GetComponent<Projectile>(), 100, Utility.PROJECTILE_OBJECTPOOL_NAME);
    }

    /// <summary>
    /// Is IEnumerator return type as to be able to stall a frame when setting up the A* grid.
    /// 
    /// IEnumerator is the data type for coroutines. We make start() a coroutine (runs in parallel)
    /// since we have a yield retunr null. If we didn't make it IEnumerator, the rest of our code 
    /// would have to wait a frame to execute making the gameplay janky
    /// </summary>
    private IEnumerator Start()
    {
        //both the methods in EventsManager are notifiers, sending updates to subscribers using observer pattern
        //we drill in from EventsManager just for these because EventsManager is part of the observer pattern
        EventsManager.instance.gameEvents.SetupNewAStarGrid(gridHeight, gridWidth, gridOffset, floorTilemap);
        yield return null; // Wait one frame to ensure all objects (Enemies, Towers) have initialized and subscribed to events

        EventsManager.instance.gameEvents.CoinsUpdated(currentCoins);
        EventsManager.instance.gameEvents.TowerGridUpdated();
    }

    public Vector3 GetEnemySpawnPoint() => enemySpawnPoint;
    public Vector3 GetEnemyGoalPoint() => enemyGoalPoint;
    public Tilemap GetFloorTilemap() => floorTilemap;
    public GameObject GetTowerPrefab() => towerPrefab;
    public GameObject GetEnemyPrefab() => enemyPrefab;
    public Vector3 GetGridOffset() => gridOffset;

    public int GetCurrentCoins() => currentCoins;
    public int GetCurrentLives() => currentLives;
    public int GetCurrentWaveIndex() => currentWaveIndex;
    public void SetCurrentWaveIndex(int wave) { currentWaveIndex = wave; }

    public bool GetGameActive() => gameActive;

    public bool TrySpendCoins(int amount) //return false if not enough coins
    {
        if (amount > currentCoins) return false;
        currentCoins -= amount;
        EventsManager.instance.gameEvents.CoinsUpdated(currentCoins);
        return true;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        EventsManager.instance.gameEvents.CoinsUpdated(currentCoins);
    }

    public void TrySubtractLife(int amount)
    {
        currentLives -= amount;
        EventsManager.instance.gameEvents.LivesUpdated(currentLives);
        if (currentLives <= 0)
        {
            EventsManager.instance.gameEvents.ToggleGameOverText();
            gameActive = false;
            // end game
        }
    }
}