using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TMPro;

public static class Utility
{

    //use layer masks instead of conditionals for performace. We could search all game objects, and say 
    // if gameObject.IsOfTypeEnemy() { do enemy stuff }, but then it searches env for all game objects 
    //BEFORE filtering. With layer masks, we can pass in the layer mask as a param, so it filters
    //BEFORE searching for game objects, so it only searches for game objects of the relevant type
    public static LayerMask FLOOR__LAYERMASK;
    public static LayerMask WALL__LAYERMASK;
    public static LayerMask TOWER__LAYERMASK;
    public static LayerMask ENEMY__LAYERMASK;

    //bounds of the level, level was designed in unity
    public const float LEVEL_BOUNDS_XMIN = -10.5f;
    public const float LEVEL_BOUNDS_XMAX = 10.5f;
    public const float LEVEL_BOUNDS_YMAX = 6.5f;
    public const float LEVEL_BOUNDS_YMIN = -6.5f;

    public const string OBJECTPOOLS_PARENT_NAME = "ObjectPools"; // parent object in hierarchy to keep all object pools organized in unity display
    public const string PROJECTILE_OBJECTPOOL_NAME = "Projectiles";

    public static void InitializeLayerMasks()
    {
        FLOOR__LAYERMASK = LayerMask.GetMask("Floor");
        WALL__LAYERMASK = LayerMask.GetMask("Wall");
        TOWER__LAYERMASK = LayerMask.GetMask("Tower");
        ENEMY__LAYERMASK = LayerMask.GetMask("Enemy");
    }


    /// <summary>
    /// Strictly a debugging function for running AStar with debugging ON. Shows the F, G and H costs of each Astar node using this function.
    /// 
    /// Creates a new empty GameObject and attaches a TextMeshPro component to it
    /// Sets its parent transform and positions it at localPosition in that parent's local space
    /// Configures the text visuals — the actual string, font size, color, sorting layer (so it renders on top of other things in the "Debugging" layer)
    /// The wide negative margin (-25, 0, -25, 0) lets the text overflow its bounding box horizontally, which is a hacky way to prevent the cost numbers from getting clipped when they're placed on small tiles
    /// Returns the TextMeshPro component so the caller can update the text later if needed
    /// </summary>
    public static TextMeshPro CreateWorldText(string objectName, Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, float localScale)
    {
        GameObject gameObject = new GameObject(objectName, typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.localScale = Vector3.one * localScale;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();

        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.sortingOrder = sortingOrder;
        textMesh.sortingLayerID = SortingLayer.NameToID("Debugging");
        textMesh.margin = new Vector4(-25, 0, -25, 0);

        return textMesh;
    }

    public static float CalculatePathLength(List<Vector3> path)
    {
        if (path == null) return 0f;
        float totalDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++) totalDistance += Vector3.Distance(path[i], path[i + 1]); //sums up distance between each pair of waypoints, path.Count is number of waypoints
        return totalDistance;
    }

    /// <summary>
    /// Snaps <paramref name="transform"/> using the game floor tilemap.
    /// </summary>
    public static void SnapToTileCenter(Transform transform)
    {
        if (transform == null || GameManager.instance == null) return;
        SnapToTileCenter(transform, GameManager.instance.GetFloorTilemap());
    }

    /// <summary>
    /// Snaps <paramref name="transform"/>'s position to the tile cell center on <paramref name="tilemap"/>.
    /// </summary>
    public static void SnapToTileCenter(Transform transform, Tilemap tilemap)
    {
        if (transform == null || tilemap == null) return;
        transform.position = SnapToTileCenter(tilemap, transform.position);
    }

    /// <summary>
    /// Snaps a world position to the center of the tile cell that contains it (uses tilemap grid, rotation, and scale).
    /// Preserves the incoming Z so 2D sorting / camera depth stay unchanged unless you assign Z yourself afterward.
    /// </summary>
    public static Vector3 SnapToTileCenter(Tilemap tilemap, Vector3 worldPosition)
    {
        if (tilemap == null) return worldPosition;
        Vector3Int cell = tilemap.WorldToCell(worldPosition);
        Vector3 center = tilemap.GetCellCenterWorld(cell);
        center.z = worldPosition.z;
        return center;
    }

    public static Vector2 RandomAngleOffset(Vector2 direction, float absOffsetDegrees)
    {
        float offset = Random.Range(-absOffsetDegrees, absOffsetDegrees) * Mathf.Deg2Rad;
        float cos = Mathf.Cos(offset);
        float sin = Mathf.Sin(offset);
        return new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );
    }

}
