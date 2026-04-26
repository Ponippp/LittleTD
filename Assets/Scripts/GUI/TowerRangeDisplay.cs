using UnityEngine;

public class TowerRangeDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private int sortingOrder = -10;

    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>(); //checks if the field was populated in the inspector
        if (_spriteRenderer == null) //checks if GetComponent found a SpriteRenderer 

        {
            Debug.LogError($"[TowerRangeDisplay] No SpriteRenderer found on {gameObject.name}");
            return;
        }

        _spriteRenderer.enabled = false; //hide the range display by default
        _spriteRenderer.sortingOrder = sortingOrder;
    }

    private void OnEnable()
    {
        Debug.Log("[TowerRangeDisplay] OnEnable - subscribing to OnTowerSelected event");
        EventsManager.instance.gameEvents.OnTowerSelected += UpdateRangeDisplay;
    }

    private void OnDisable()
    {
        Debug.Log("[TowerRangeDisplay] OnDisable - unsubscribing from OnTowerSelected event");
        EventsManager.instance.gameEvents.OnTowerSelected -= UpdateRangeDisplay;
    }

    private void UpdateRangeDisplay(Tower selectedTower)
    {
        Debug.Log($"[TowerRangeDisplay] UpdateRangeDisplay called with tower: {selectedTower?.name}");
        if (_spriteRenderer == null)
        {
            Debug.LogError("[TowerRangeDisplay] SpriteRenderer is missing at runtime.");
            return;
        }

        if (selectedTower != null)
        {
            float range = selectedTower.GetTowerRange();
            Debug.Log($"[TowerRangeDisplay] Tower range: {range}, enabling sprite renderer");
            transform.position = selectedTower.transform.position; // Position at the tower
            transform.localScale = new Vector3(range * 2, range * 2, 1); // Scale to match the tower's range (range is radius, so multiply by 2 to get diameter)
            _spriteRenderer.enabled = true;
        }
        else
        {
            Debug.Log($"[TowerRangeDisplay] Selected tower is null, disabling sprite renderer");
            _spriteRenderer.enabled = false;
        }
    }
}