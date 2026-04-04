using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpriteLoader : MonoBehaviour
{
    public static SpriteLoader instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public List<Sprite> LoadTowerSprites(string towerName, string state)
    {
        string targetAssetName = $"{towerName}3D_{state}";

        // Priority 1: Specific path seen in project structure
        // Sprites/Towers/{towerName}/{towerName}3D_{state}
        string specificPath = $"Sprites/Towers/{towerName}/{targetAssetName}";
        Sprite[] sprites = Resources.LoadAll<Sprite>(specificPath);

        // Priority 2: Broad search if not found in specific path
        if (sprites == null || sprites.Length == 0)
        {
            // If we don't know the path, we "search" the resources.
            Sprite[] allSprites = Resources.LoadAll<Sprite>("");
            sprites = allSprites.Where(s => s.name.StartsWith(targetAssetName)).ToArray();
        }

        // Sort sprites numerically by their name (e.g., "Frame_1", "Frame_2", "Frame_10" -> 1, 2, 10)
        // This ensures they are in the correct order for animations.
        return sprites
            .OrderBy(s => {
                var match = System.Text.RegularExpressions.Regex.Match(s.name, @"(\d+)$");
                return match.Success ? int.Parse(match.Value) : 0;
            })
            .ToList();
    }
}
