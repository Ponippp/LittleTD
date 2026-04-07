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
        // Path should be relative to Resources, no extension
        string specificPath = $"Sprites/Towers/{towerName}/{targetAssetName}";
        
        // Resources.LoadAll on a specific file should return its sub-assets (the sprites)
        Sprite[] sprites = Resources.LoadAll<Sprite>(specificPath);

        if (sprites != null && sprites.Length > 0)
        {
            return sprites.OrderBy(s => ExtractNumber(s.name)).ToList();
        }

        // Broad fallback if specific load fails
        string folderPath = $"Sprites/Towers/{towerName}";
        Sprite[] folderSprites = Resources.LoadAll<Sprite>(folderPath);
        
        // If they are just named "Frame_X", we have to guess or use the first half? 
        // This usually means the specific load above should have worked if the importer is active.
        return folderSprites
            .Where(s => s.name.Contains(targetAssetName) || s.name.Contains(state))
            .OrderBy(s => ExtractNumber(s.name))
            .ToList();
    }

    private int ExtractNumber(string name)
    {
        // Handles "Frame_1", "Giga_1", or just "1"
        var match = System.Text.RegularExpressions.Regex.Match(name, @"(\d+)$");
        return match.Success ? int.Parse(match.Value) : 0;
    }
}
