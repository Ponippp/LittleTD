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

    public struct EnemyAnimations
    {
        public List<Sprite> runUp;
        public List<Sprite> runDown;
        public List<Sprite> runRight; 
    }

    public EnemyAnimations LoadEnemySprites(string enemyName, int[] downRange, int[] upRange, int[] rightRange)
    {
        string folderPath  = $"Sprites/Enemies/{enemyName}";
        Sprite[] all       = Resources.LoadAll<Sprite>(folderPath);

        if (all == null || all.Length == 0)
        {
            return new EnemyAnimations();
        }

        var frameMap = all
            .Where(s => s.name.StartsWith("Frame_"))
            .ToDictionary(s => ExtractTrailingNumber(s.name), s => s);

        return new EnemyAnimations
        {
            runDown  = SliceRange(frameMap, downRange[0],  downRange[1],  "RUN_DOWN"),
            runUp    = SliceRange(frameMap, upRange[0],    upRange[1],    "RUN_UP"),
            runRight = SliceRange(frameMap, rightRange[0], rightRange[1], "RUN_RIGHT"),
        };
    }

    private List<Sprite> SliceRange(Dictionary<int, Sprite> frameMap, int from, int to, string label)
    {
        var result = new List<Sprite>();
        for (int i = from; i <= to; i++)
        {
            if (frameMap.TryGetValue(i, out Sprite s))
                result.Add(s);
        }

        return result;
    }

    private int ExtractTrailingNumber(string name)
    {
        var match = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)$");
        return match.Success ? int.Parse(match.Value) : 0;
    }
}
