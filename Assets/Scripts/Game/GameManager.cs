using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private Vector3 gridOffset = Vector3.zero;
    [SerializeField] private Vector3 enemySpawnPoint;
    [SerializeField] private Vector3 enemyGoalPoint;
    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
        if (floorTilemap == null) { floorTilemap = FindAnyObjectByType<Tilemap>(); }
        Utility.InitializeLayerMasks();
    }
    private void Start()
    {
        EventsManager.instance.gameEvents.SetupNewAStarGrid(gridHeight, gridWidth, gridOffset, floorTilemap);
    }

    public Vector3 GetEnemySpawnPoint() => enemySpawnPoint;
    public Vector3 GetEnemyGoalPoint() => enemyGoalPoint;
}