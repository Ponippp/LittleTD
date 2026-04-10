using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gets sprites from Resources folder into arrays within GameObjects
/// </summary>
public class SpriteLoader : MonoBehaviour
{
    public static SpriteLoader instance { get; private set; }

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }

    public List<Sprite> LoadTowerSprites(string towerName, string state)
    {
        //access+load sprites from files
        string targetAssetName = $"{towerName}3D_{state}";
        string specificPath = $"Sprites/Towers/{towerName}/{targetAssetName}";

        Sprite[] sprites = Resources.LoadAll<Sprite>(specificPath);

        if (sprites != null && sprites.Length > 0)
            return sprites.OrderBy(s => ExtractTrailingNumber(s.name)).ToList();

        // Broad fallback
        string folderPath = $"Sprites/Towers/{towerName}";
        Sprite[] folderSprites = Resources.LoadAll<Sprite>(folderPath);

        return folderSprites
            .Where(s => s.name.Contains(targetAssetName) || s.name.Contains(state))
            .OrderBy(s => ExtractTrailingNumber(s.name))
            .ToList();
    }

    public struct EnemyRunClips
    {
        public AnimationClip runDown;
        public AnimationClip runUp;
        public AnimationClip runRight;

        public AnimationClip AnyNonNull() => runDown ?? runUp ?? runRight;
    }

    /// <summary>
    /// Loads AnimationClips from Resources/Sprites/Enemies/&lt;enemyName&gt;.
    /// Prefer Aseprite tags (or clip names) containing Enemy_RUN_DOWN, Enemy_RUN_UP, Enemy_RUN_RIGHT (or RUN_DOWN / RUN_UP / RUN_RIGHT).
    /// If the asset exports exactly one clip, it is used for all directions.
    /// </summary>
    public EnemyRunClips LoadEnemyRunClips(string enemyName)
    {
        string path = $"Sprites/Enemies/{enemyName}";
        AnimationClip[] all = Resources.LoadAll<AnimationClip>(path);
        var result = new EnemyRunClips();

        if (all == null || all.Length == 0)
            return result;

        if (all.Length == 1)
        {
            result.runDown = result.runUp = result.runRight = all[0];
            return result;
        }

        foreach (AnimationClip clip in all)
        {
            string u = clip.name.ToUpperInvariant();
            if (u.Contains("RUN_DOWN") || u.Contains("ENEMY_RUN_DOWN"))
                result.runDown = clip;
            else if (u.Contains("RUN_UP") || u.Contains("ENEMY_RUN_UP"))
                result.runUp = clip;
            else if (u.Contains("RUN_RIGHT") || u.Contains("ENEMY_RUN_RIGHT") || u.Contains("RUN_LEFT") || u.Contains("ENEMY_RUN_LEFT"))
                result.runRight = clip;
        }

        if (result.runDown == null) result.runDown = result.runUp ?? result.runRight;
        if (result.runUp == null) result.runUp = result.runDown ?? result.runRight;
        if (result.runRight == null) result.runRight = result.runDown ?? result.runUp;

        return result;
    }

    private int ExtractTrailingNumber(string name)
    {
        var match = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)$");
        return match.Success ? int.Parse(match.Value) : 0;
    }
}
