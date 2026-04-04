using UnityEngine;
using System.Collections.Generic;
using TMPro;

public static class Utility
{

    public static LayerMask FLOOR__LAYERMASK;
    public static LayerMask WALL__LAYERMASK;
    public static LayerMask TOWER__LAYERMASK;

    public static void InitializeLayerMasks()
    {
        FLOOR__LAYERMASK = LayerMask.GetMask("Floor");
        WALL__LAYERMASK = LayerMask.GetMask("Wall");
        TOWER__LAYERMASK = LayerMask.GetMask("Tower");
    }

    public static TextMeshPro CreateWorldText(string objectName, Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, float localScale)
    {
        GameObject gameObject = new GameObject(objectName, typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.localScale = Vector3.one * localScale; // TMP has its own sizing
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();

        // Set alignment and anchoring (TMP has different property names)
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
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(path[i], path[i + 1]);
        }
        return totalDistance;
    }


}