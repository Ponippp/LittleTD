using UnityEngine;

public class Projectile : MonoBehaviour
{
    // [SerializeField] private SpriteRenderer spriteRenderer;
    public float speed = 0;
    public float damage = 0;

    private IProjectileMovementStrategy _strategy;

    // private void Awake()
    // {
    //     spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    // }

    private System.Action<float> _onHitCallback;

    public void Initialize(float damage, float speed, IProjectileMovementStrategy strategy, System.Action<float> onHitCallback = null)
    {
        this.damage = damage;
        this.speed = speed;
        this._strategy = strategy;
        this._onHitCallback = onHitCallback;
    }


    // public void SetAimingStrategy(IAimingStrategy strategy)
    // {
    //     _strategy = strategy;
    // }

    void Update()
    {
        if (_strategy == null) return;

        if (ProjectileOutOfBounds())
        {
            ResetAndEnqueueProjectile();
            return;
        }

        Vector3 oldPosition = transform.position;
        _strategy.Move();
        Vector3 newPosition = transform.position;

        RotateToFaceMovementDirection((newPosition - oldPosition).normalized);
    }

    private bool ProjectileOutOfBounds()
    {
        Vector3 pos = transform.position;
        return pos.x < Utility.LEVEL_BOUNDS_XMIN || pos.x > Utility.LEVEL_BOUNDS_XMAX || pos.y < Utility.LEVEL_BOUNDS_YMIN || pos.y > Utility.LEVEL_BOUNDS_YMAX;
    }

    private void RotateToFaceMovementDirection(Vector3 moveDir)
    {
        if (moveDir != Vector3.zero) transform.up = -moveDir;
    }

    // weird syntax to make this function more efficient since it is used a ton
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & Utility.ENEMY__LAYERMASK) != 0)
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
                _onHitCallback?.Invoke(damage); //?: if onhitcallback !=null, then invoke
                ResetAndEnqueueProjectile();
            }
        }
    }

    public void ResetAndEnqueueProjectile()
    {
        Reset();
        ObjectPooler.EnqueueObject(this, Utility.PROJECTILE_OBJECTPOOL_NAME);
    }

    private void Reset()
    {
        speed = 0;
        damage = 0;
    }

    public float GetSpeed() => speed;
}

