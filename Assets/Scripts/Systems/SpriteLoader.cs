using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        if (sprites == null || sprites.Length == 0)
        {
            Sprite[] allSprites = Resources.LoadAll<Sprite>("");
            sprites = allSprites.Where(s => s.name.StartsWith(targetAssetName)).ToArray();
        }

        return sprites
            .OrderBy(s =>
            {
                var match = System.Text.RegularExpressions.Regex.Match(s.name, @"(\d+)$");
                return match.Success ? int.Parse(match.Value) : 0;
            })
            .ToList();
    }
}
