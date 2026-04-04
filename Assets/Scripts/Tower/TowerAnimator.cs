using System.Collections.Generic;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
    [SerializeField] private string towerName;
    [SerializeField] private float shootAnimationTime = 0.3f;
    [SerializeField] private List<Sprite> idleSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> shootSprites = new List<Sprite>();
    
    private Tower tower;
    private float shootTimer = 0f;

    private void Awake()
    {
        tower = GetComponentInParent<Tower>();
        if (tower == null) Debug.LogError("[TowerAnimator] No Tower component found on this GameObject.");
    }

    private void OnEnable()
    {
        if (tower != null) tower.OnFire += HandleFire;
    }

    private void OnDisable()
    {
        if (tower != null) tower.OnFire -= HandleFire;
    }

    private void HandleFire()
    {
        shootTimer = shootAnimationTime;
    }

    private void Start()
    {
        idleSprites = SpriteLoader.instance.LoadTowerSprites(towerName, "IDLE");
        shootSprites = SpriteLoader.instance.LoadTowerSprites(towerName, "SHOOT");
    }

    private void Update()
    {
        if (tower == null) return;

        if (shootTimer > 0f) shootTimer -= Time.deltaTime;

        List<Sprite> activeSet = ResolveActiveSet();
        if (activeSet == null || activeSet.Count == 0) return;

        float angle = tower.GetLookingDirection();
        Sprite chosen = SelectSprite(activeSet, angle);
        tower.SetSprite(chosen);
    }

    private List<Sprite> ResolveActiveSet()
    {
        if (shootTimer > 0f) return shootSprites;
        return idleSprites;
    }

    /// <summary>
    /// Select sprite based on the angle the tower is looking in.
    /// </summary>
    private static Sprite SelectSprite(List<Sprite> sprites, float angle)
    {
        if (sprites.Count <= 1) return sprites.Count == 0 ? null : sprites[0];

        angle = ((angle + 90f) % 360f + 360f) % 360f;

        float indexPerDegree = sprites.Count / 360f;
        int index = Mathf.RoundToInt(angle * indexPerDegree);
        
        // Wrap index around
        index %= sprites.Count;

        return sprites[index];
    }

}