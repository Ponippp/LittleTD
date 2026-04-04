using System;
using UnityEngine;

public static class Utility{

        public static LayerMask WALL__LAYERMASK;
        public static LayerMask OBSTACLE_INTERACTABLE__LAYERMASK;
public static TextMesh CreateWorldText(string objectName, Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, float localScale)
    {
        GameObject gameObject = new GameObject(objectName, typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.localScale = transform.localScale * localScale;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        textMesh.GetComponent<MeshRenderer>().sortingLayerName = "Debugging";
        return textMesh;
    }
}