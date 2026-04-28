using System;
using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private SpriteRenderer[] spriteRenderers;

    [Header("Runtime Stats (Auto-filled by Factory/Configure)")]
    [SerializeField] private TowerStats stats = new();
    [Serializable]
    public class TowerStats
    {
        public BaseBoostedFloat range = new();
        public BaseBoostedFloat fireInterval = new();
        public BaseBoostedFloat baseBulletSpreadAngle = new(); //since BaseBoostedFloat is a custom data type (a class under the hood), need to create a new instance of it or it will be null
        public BaseBoostedInt projectilesFiredWithEachShot = new();
        public BaseBoostedFloat baseReleaseTimeBetweenEachProjectileInBurst = new();
        public BaseBoostedFloat currentTowerSellValue = new(); // affected by discounts, not equal to baseTowerCost
        public float fireCooldown = 0f;
        public Aiming aiming = new();
        [Serializable] //makes public class Aiming serializable, so you can edit the whole thing in inspector
        public class Aiming
        {
            public IAimingStrategy strategy;
            public AimingResult currentResult;
            public TowerAimingType type = TowerAimingType.FIRST;
            public BaseBoostedFloat swivelSpeed = new();
            // public BaseBoostedFloat aimingWindowWhereTowerCanShootAtEnemyRadians = new();
        }
        public Projectile projectile = new();
        [Serializable]
        public class Projectile
        {
            public BaseBoostedFloat speed = new();
            public BaseBoostedFloat damage = new();
            public ProjectileMovementType movementType = ProjectileMovementType.DIRECTED;
        }

        public Visual visual = new();
        [Serializable]
        public class Visual
        {
            public float fireAnimationTime = 0.1f;
            public Vector2 projectileSpawnRingBottomOffset = new Vector2(0f, -0.2f);
            public float projectileSpawnRingRadius = 0.75f;
            public float lastLookingAngle;
        }
        public Record record = new();
        [Serializable]
        public class Record
        {
            public float totalDamageDealt = 0;
            public string towerName = "";
            public string towerDescription = "";
            public BaseBoostedFloat baseTowerCost = new();
            public TowerState towerState = TowerState.IDLE;
            public TowerType towerType;
            public bool isInitalized = false;
        }
    }
    public event Action OnFire;


    private void Awake()
    {
        /*
        A Tower prefab can have multiple child GameObjects each with their own SpriteRenderer.
        for example all the diff tower angles and the range sprite. GetComponentsInChildren crawls the
        entire hierarchy and collects all of them into an array so you can control them all at once.
        */
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("[Tower] No SpriteRenderer components found on this GameObject or its children.");
            return;
        }

        spriteRenderer = spriteRenderers[0]; //grabs the first sprite renderer it finds as the main one for things like changing sorting order, but we can still change the sprite or color of all of them with SetSprite and SetColor functions that loop through the array
    }

    public void Initialize(TowerStats newStats)
    {
        stats = newStats;
        stats.fireCooldown = 0f;

        stats.aiming.strategy = GetAimingType();

        gameObject.name = stats.record.towerName;
        stats.record.isInitalized = true;
    }

    private IAimingStrategy GetAimingType()
    {
        return stats.aiming.type switch
        {
            TowerAimingType.FIRST => new FirstEnemyStrategy(),
            TowerAimingType.CLOSEST => new ClosestEnemyStrategy(),
            TowerAimingType.STRONGEST => new StrongestEnemyStrategy(),
            TowerAimingType.LAST => new LastEnemyStrategy(),
            TowerAimingType.WEAKEST => new WeakestEnemyStrategy(),
            TowerAimingType.SPIN => new SpinStrategy(stats.aiming.swivelSpeed.BaseBoostedF),
            _ => new FirstEnemyStrategy(),
        };
    }

    private void Update()
    {
        if (!stats.record.isInitalized || !GameManager.instance.GetGameActive()) return; //need to have tower data initialized before updateAiming each frame b/c update aiming relies on tower fire rate/cooldown
        UpdateAiming();
    }

    private void UpdateAiming()
    {
        if (stats.fireCooldown > 0f) stats.fireCooldown -= Time.deltaTime;

        AimingResult result = stats.aiming.strategy.UpdateAiming(transform.position, stats.range.BaseBoostedF); //give me the base range + boosted range of the tower (BaseBoostedF is like getBaseBoostedFloat())

        bool aimFromStrategy = stats.aiming.type == TowerAimingType.SPIN || result.enemy != null;
        if (aimFromStrategy) stats.visual.lastLookingAngle = result.lookingAngle;
        else result.lookingAngle = stats.visual.lastLookingAngle;

        stats.aiming.currentResult = result;

        if (stats.aiming.currentResult.shouldFire && stats.fireCooldown <= 0f)
        {
            Fire(stats.aiming.currentResult);
            //reset fireCooldown
            stats.fireCooldown = stats.fireInterval.BaseBoostedF; //base value is boost(0). if we wanted to update the cooldown from 2s to 1s, we would say cooldown -= 1, which is like taking stats.fireInterval.boost() and making the boost -1 instead of the default of 0
        }
    }

    private void Fire(AimingResult result)
    {
        OnFire?.Invoke();
        StartCoroutine(FireCoroutine(stats.projectile.damage.BaseBoostedF, stats.projectile.speed.BaseBoostedF, stats.projectile.movementType, result));
    }

    //coroutine runs in parallel so you don’t have to wait for it to execute before executing 
    //other code. Useful when you have a function that has pauses like 
    //yield return null (stalls 1 frame) or yield return WaitForSeconds(seconds)
    private IEnumerator FireCoroutine(float dmg, float speed, ProjectileMovementType movementType, AimingResult result)
    {
        for (int i = 0; i < stats.projectilesFiredWithEachShot.BaseBoostedI; i++)
        {
            Vector3 spawnPos = CalculateProjectileSpawnPosition(result.targetPosition); //purely for art; we want to spawn the projectile on the gun nozzle, and the gun nozzle is a different size+shape for different towers

            Projectile proj = ObjectPooler.DequeueObject<Projectile>(Utility.PROJECTILE_OBJECTPOOL_NAME);
            proj.gameObject.SetActive(true); //grabs inactive object from pool and activates it
            proj.transform.position = spawnPos;

            if (movementType == ProjectileMovementType.HOMING && result.enemy != null)
            {
                proj.Initialize(dmg, speed, new HomingStrategy(proj, result.enemy), RecordDamageDealt);
            }
            else
            {
                Vector2 baseDir = (result.targetPosition - spawnPos).normalized;
                if (CheckIfTargetWithinProjectileSpawnCircle(result.targetPosition)) baseDir = (result.targetPosition - transform.position).normalized; // override if within fire radius to avoid shooting backwards
                Vector2 spreadDir = Utility.RandomAngleOffset(baseDir, stats.baseBulletSpreadAngle.BaseBoostedF);
                proj.Initialize(dmg, speed, new DirectionalStrategy(proj, spreadDir), RecordDamageDealt);
            }

            yield return new WaitForSeconds(stats.baseReleaseTimeBetweenEachProjectileInBurst.BaseBoostedF);
        }
    }

    private Vector3 CalculateProjectileSpawnPosition(Vector3 target)
    {
        Vector3 bottomPos = transform.position + (Vector3)stats.visual.projectileSpawnRingBottomOffset;
        Vector3 ringCenter = bottomPos + (Vector3.up * stats.visual.projectileSpawnRingRadius);

        Vector3 targetDir = (target - transform.position).normalized;

        return ringCenter + (targetDir * stats.visual.projectileSpawnRingRadius);
    }

    private bool CheckIfTargetWithinProjectileSpawnCircle(Vector3 target)
    {
        Vector3 bottomPos = transform.position + (Vector3)stats.visual.projectileSpawnRingBottomOffset;
        Vector3 ringCenter = bottomPos + (Vector3.up * stats.visual.projectileSpawnRingRadius);

        return Vector3.Distance(target, ringCenter) <= stats.visual.projectileSpawnRingRadius;
    }

    public float GetLookingDirection() => stats.aiming.currentResult.lookingAngle;

    public TowerState GetTowerState() { return stats.record.towerState; }
    public TowerType GetTowerType() { return stats.record.towerType; }
    public string GetTowerName() { return stats.record.towerName; }
    public bool GetIsInitalized() { return stats.record.isInitalized; }
    public float GetTowerRange() => stats.range.BaseBoostedF;

    public void RecordDamageDealt(float damage) { stats.record.totalDamageDealt += damage; }

//setSprite and setColor functions loop through all sprite renderers on the tower and its children so that we can change the sprite or color of the entire tower at once
    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderers == null) return;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null) spriteRenderer.sprite = sprite;
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderers == null) return;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null) spriteRenderer.color = color;
        }
    }

    private void OnMouseDown()
    {
        //debugging:
        // Debug.Log($"[Tower] Tower clicked: {gameObject.name}");
        // // Temporary visual feedback - change color to red when clicked
        // SetColor(Color.red);
        // // Reset color after 0.5 seconds
        // StartCoroutine(ResetColorAfterDelay(0.5f));
        //

        EventsManager.instance.gameEvents.TowerSelected(this);
    }

    //debugging fucntion
    // private System.Collections.IEnumerator ResetColorAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     SetColor(Color.white); // Assuming default color is white
    // }

}
