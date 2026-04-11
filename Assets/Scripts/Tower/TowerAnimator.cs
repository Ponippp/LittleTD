using System.Collections.Generic;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
    [SerializeField] private float shootAnimationTime = 0.1f;
    [SerializeField] private List<Sprite> idleSprites = new();
    [SerializeField] private List<Sprite> shootSprites = new();

    private Tower tower;
    private float shootTimer = 0f;
    private bool spritesLoaded = false;

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

    private void Start() => TryLoadSprites();

    private void Update()
    {
        if (tower == null) return;

        // Ensure sprites are loaded once tower is initialized
        if (!spritesLoaded && tower.GetIsInitalized()) TryLoadSprites();

        if (shootTimer > 0f) shootTimer -= Time.deltaTime;

        List<Sprite> activeSet = ResolveActiveSet();
        if (activeSet == null || activeSet.Count == 0) return;

        float angle = tower.GetLookingDirection();
        //there are 64 sprites for each tower, so it looks good when rotating, so this selects the sprite that is closest to the direction the tower is facing
        Sprite chosen = SelectSprite(activeSet, angle);
        tower.SetSprite(chosen);
    }

    private void TryLoadSprites()
    {
        string towerName = tower.GetTowerName();
        if (string.IsNullOrEmpty(towerName)) return;

        idleSprites = SpriteLoader.instance.LoadTowerSprites(towerName, "IDLE");
        shootSprites = SpriteLoader.instance.LoadTowerSprites(towerName, "SHOOT");

        if (idleSprites.Count > 0) spritesLoaded = true;
    }

    private List<Sprite> ResolveActiveSet()
    {
        if (shootTimer > 0f) return shootSprites;
        return idleSprites;
    }

    private static Sprite SelectSprite(List<Sprite> sprites, float angle)
    {
        //if no sprites, return null, if 1 sprite, return that sprite
        if (sprites.Count <= 1) return sprites.Count == 0 ? null : sprites[0];

        // Adjust angle for sprite orientation (assuming 0 is Right, +90 is Up)
        angle = ((angle + 90f) % 360f + 360f) % 360f;

        float indexPerDegree = sprites.Count / 360f;
        int index = Mathf.RoundToInt(angle * indexPerDegree);

        index %= sprites.Count;
        return sprites[index];
    }
}