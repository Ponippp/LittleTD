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
        //check if its null before checking length. if we just checked length and sprites was null, it would do null.length and compile time error
        if (sprites != null && sprites.Length > 0) //with both conditions, it goes to broad fallback if sprites is null
            return sprites.OrderBy(s => ExtractTrailingNumber(s.name)).ToList(); //if you had files called sprite1 thru sprite 50, this returns a list of them like [Sprite1,...,Sprite50]

        // Broad fallback: files are in format TOWER_NAME3D_FIRE or _IDLE, so this just looks for TOWER_NAME in files in case they're not is correct format for some reason
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
    public EnemyRunClips LoadEnemyRunClips(string enemyName) //automatically loads all types of enemy animations so we dont have to manually drag all the different animations in unity editor upon creating a new enemy
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

        foreach (AnimationClip clip in all) //array of sprites with other features that we dont use
        {
            string u = clip.name.ToUpperInvariant();
            if (u.Contains("RUN_DOWN") || u.Contains("ENEMY_RUN_DOWN"))
                result.runDown = clip;
            else if (u.Contains("RUN_UP") || u.Contains("ENEMY_RUN_UP"))
                result.runUp = clip;
            else if (u.Contains("RUN_RIGHT") || u.Contains("ENEMY_RUN_RIGHT") || u.Contains("RUN_LEFT") || u.Contains("ENEMY_RUN_LEFT"))
                result.runRight = clip;
        }

        if (result.runDown == null) result.runDown = result.runUp ?? result.runRight; //null coalescing operator ??: if run up is null, sets it to run right. if not null, sets it to run up
        if (result.runUp == null) result.runUp = result.runDown ?? result.runRight;
        if (result.runRight == null) result.runRight = result.runDown ?? result.runUp;

        return result;
    }

    private int ExtractTrailingNumber(string name)
    {
        var match = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)$");
        return match.Success ? int.Parse(match.Value) : 0; //since we're only using this on sprites, which are automatically named based on the frame (ie frame 1 sprite is sprite_1), this will never return 0. but we have it in case we want to call it on different objects down the line
    }
}
