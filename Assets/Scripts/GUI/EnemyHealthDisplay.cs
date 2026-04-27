using UnityEngine;

public class EnemyHealthDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private int sortingOrder = 100;
    [SerializeField] private Vector3 offset = new Vector3(0, -0.5f, 0); // Position below enemy
    [SerializeField] private float barWidth = 1f; // Full health bar width
    [SerializeField] private float barHeight = 0.1f; // Bar height

    private Enemy _enemy;
    private float _maxHealth;
    private bool _isInitialized;

    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError($"[EnemyHealthDisplay] No SpriteRenderer found on {gameObject.name}");
            return;
        }

        _spriteRenderer.sortingOrder = sortingOrder;
        _spriteRenderer.enabled = true; // keep the bar visible once initialized

        // Find the enemy component (should be on parent or same object)
        _enemy = GetComponentInParent<Enemy>();
        if (_enemy == null) _enemy = GetComponent<Enemy>();

        if (_enemy == null)
        {
            Debug.LogError($"[EnemyHealthDisplay] No Enemy component found on {gameObject.name} or its parents");
            return;
        }

        Debug.Log($"[EnemyHealthDisplay] Found enemy {_enemy.name}, waiting for initialization");
    }

    private void Update() //want to reposition and adjust health bar every frame
    {
        if (_enemy == null) return;

        // Initialize on first frame after enemy is fully initialized
        if (!_isInitialized)
        {
            if (!_enemy.GetIsInitialized()) return; // Wait for enemy to initialize

            _maxHealth = _enemy.GetHealth();
            if (_maxHealth <= 0f) _maxHealth = 1f;
            _isInitialized = true;
            Debug.Log($"[EnemyHealthDisplay] Initialized for enemy {_enemy.name} with max health {_maxHealth}");
        }

        // Position below the enemy
        transform.position = _enemy.transform.position + offset;

        float currentHealth = _enemy.GetHealth();
        float healthPercent = Mathf.Clamp01(currentHealth / _maxHealth); //clamp01 restricts the value b/t 0 and 1

        //adjust healthbar based on health percentage
        transform.localScale = new Vector3(Mathf.Max(0.01f, barWidth * healthPercent), barHeight, 1f);

        if (healthPercent > 0.6f)
            _spriteRenderer.color = Color.green;
        else if (healthPercent > 0.3f)
            _spriteRenderer.color = Color.yellow;
        else
            _spriteRenderer.color = Color.red;

        // Hide if enemy is dead (health <= 0)
        _spriteRenderer.enabled = currentHealth > 0;
    }
}



