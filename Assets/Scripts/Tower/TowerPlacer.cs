using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TowerType defaultTowerType = TowerType.GIGA_GATLING;

    private Tower _ghostTower;
    private AStar _astar;
    private Camera _mainCamera;
    private Vector3Int _lastGridPos = new Vector3Int(-999, -999, -999);
    private bool _lastValidity = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _astar = FindAnyObjectByType<AStar>();
        if (_astar == null) Debug.LogError("[TowerPlacer] AStar component not found in scene.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) TogglePlacementMode();

        if (_ghostTower != null)
        {
            UpdateGhostPosition();

            Vector3Int currentGridPos = GameManager.instance.GetFloorTilemap().WorldToCell(_ghostTower.transform.position);
            if (currentGridPos != _lastGridPos)
            {
                _lastGridPos = currentGridPos;
                _lastValidity = CheckPlacementValidity();
            }

            _ghostTower.SetColor(_lastValidity ? Color.white : Color.red);

            if (_lastValidity && Input.GetMouseButtonDown(0)) PlaceTower();

            if (Input.GetKeyDown(KeyCode.Escape)) CancelPlacement();
        }
    }

    private void TogglePlacementMode()
    {
        if (_ghostTower == null) StartPlacement(defaultTowerType);
        else CancelPlacement();
    }

    private void StartPlacement(TowerType type)
    {
        Tower prefab = GameManager.instance.GetTowerPrefab().GetComponent<Tower>();
        if (prefab == null) return;

        _ghostTower = Instantiate(prefab);
        _ghostTower.gameObject.layer = LayerMask.NameToLayer("Tower");
        UpdateGhostPosition();
    }

    private void UpdateGhostPosition()
    {
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 gridOffset = GameManager.instance.GetGridOffset();

        // Find indices
        int xIndex = Mathf.FloorToInt(mousePos.x - gridOffset.x);
        int yIndex = Mathf.FloorToInt(mousePos.y - gridOffset.y);

        // Snap to center
        Vector3 snappedPos = new Vector3(xIndex + 0.5f, yIndex + 0.5f, 0) + gridOffset;
        _ghostTower.transform.position = snappedPos;
    }

    /// <summary>
    /// Must be on Floor tile, NOT on Wall tile, NOT overlapping another tower, and must NOT block all paths from spawn to goal.
    /// </summary>
    private bool CheckPlacementValidity()
    {
        Vector3 pos = _ghostTower.transform.position;

        // Sync transforms so physics queries see the tower at its new snapped position
        Physics2D.SyncTransforms();

        // 1. Check Floor
        Tilemap floor = GameManager.instance.GetFloorTilemap();
        Vector3Int cellPos = floor.WorldToCell(pos);
        if (!floor.HasTile(cellPos)) return false;

        // 2. Check overlap with walls (Explicit check)
        if (Physics2D.OverlapCircle(pos, 0.2f, Utility.WALL__LAYERMASK)) return false;

        // 3. Check overlap with other towers
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, 0.2f, Utility.TOWER__LAYERMASK);
        foreach (var col in overlaps)
        {
            if (col.gameObject != _ghostTower.gameObject) return false;
        }

        // 4. Protection for Spawn and Goal
        Vector3 start = GameManager.instance.GetEnemySpawnPoint();
        Vector3 end = GameManager.instance.GetEnemyGoalPoint();

        // Don't allow placing directly on start or end points
        if (Vector3.Distance(pos, start) < 0.5f || Vector3.Distance(pos, end) < 0.5f) return false;

        AStarNode startNode = _astar.GetGridNode(start);
        AStarNode endNode = _astar.GetGridNode(end);

        if (startNode == null || endNode == null) return false;

        // 5. Check AStar Path
        var path = _astar.TryRunAStar(startNode, endNode);
        if (path == null || path.Count == 0) return false;

        return true;
    }

    private void PlaceTower()
    {
        TowerData data = TowerFactory.Instance.GetDataByType(defaultTowerType);
        _ghostTower.Initialize(TowerFactory.Instance.CreateTowerStats(data));
        _ghostTower = null;
        _lastGridPos = new Vector3Int(-999, -999, -999);
        EventsManager.instance.gameEvents.TowerGridUpdated();
    }

    private void CancelPlacement()
    {
        if (_ghostTower != null)
        {
            Destroy(_ghostTower.gameObject);
            _ghostTower = null;
            _lastGridPos = new Vector3Int(-999, -999, -999);
        }
    }
}
