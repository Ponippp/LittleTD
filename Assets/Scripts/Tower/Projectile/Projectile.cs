using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    // [SerializeField] private SpriteRenderer spriteRenderer;
    public float speed = 0;
    public float damage = 0;
    public int pierce = 0;
    // private ProjectileData data;

    private IProjectileMovementStrategy _strategy;

    // private void Awake()
    // {
    //     spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    // }

    private Action<float> _onHitCallback;

    public void Initialize(float damage, float speed, IProjectileMovementStrategy strategy, System.Action<float> onHitCallback = null)
    {
        this.damage = damage;
        this.speed = speed;
        this.pierce = 1; // TODO REMOVE PLACEHOLDER REPLACE WITH ProjectileData
        _strategy = strategy;
        _onHitCallback = onHitCallback;
    }

    void Update()
    {
        if (_strategy == null) return;

        if (ProjectileOutOfBounds())
        {
            ResetAndEnqueueProjectile(); //resets projectile's speed+dmg to 0 and returns it to the object pool

            return;
        }

        Vector3 oldPosition = transform.position;
        _strategy.Move(); //either moves in a straight line or homes in on an enemy, depending on the strategy.
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
        //moveDir is the vector from tower to enemy
        //transform.up points north for the tower. This is not necessarily global north.
        //we rotate the projectile vector to face the tower's south b/c the sprite art is made so
        //the nozzle of the tower faces south
        if (moveDir != Vector3.zero) transform.up = -moveDir;
    }

    // weird syntax to make this function more efficient since it is used a ton
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pierce <= 0) { ResetAndEnqueueProjectile(); return; } // safety check, should never happen
        if (((1 << other.gameObject.layer) & Utility.ENEMY__LAYERMASK) != 0)
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                pierce--;
                enemy.TakeDamage(damage);
                _onHitCallback?.Invoke(damage); //"?" means if onhitcallback !=null, then invoke

                if (pierce <= 0) ResetAndEnqueueProjectile();
            }
        }
    }

    public void ResetAndEnqueueProjectile()
    {
        Reset();
        //this is the instance of the object we want to reset+enqueue, we specify the name of the object pool 
        //so we queue it into the right one
        ObjectPooler.EnqueueObject(this, Utility.PROJECTILE_OBJECTPOOL_NAME);
    }

    private void Reset()
    {
        speed = 0;
        damage = 0;
        pierce = 0;
    }

    public float GetSpeed() => speed;
}

