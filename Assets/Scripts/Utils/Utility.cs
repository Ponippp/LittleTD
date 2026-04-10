using UnityEngine;
using System.Collections.Generic;
using TMPro;

public static class Utility
{

    public static LayerMask FLOOR__LAYERMASK;
    public static LayerMask WALL__LAYERMASK;
    public static LayerMask TOWER__LAYERMASK;
    public static LayerMask ENEMY__LAYERMASK;

    public const float LEVEL_BOUNDS_XMIN = -10.5f;
    public const float LEVEL_BOUNDS_XMAX = 10.5f;
    public const float LEVEL_BOUNDS_YMAX = 6.5f;
    public const float LEVEL_BOUNDS_YMIN = -6.5f;

    public const string OBJECTPOOLS_PARENT_NAME = "ObjectPools";
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
        for (int i = 0; i < path.Count - 1; i++) totalDistance += Vector3.Distance(path[i], path[i + 1]);
        return totalDistance;
    }

    // /// <summary>
    // /// Overrides a clip in the provided AnimatorOverrideController and plays the 'IDLE' state.
    // /// EDITED: Now accepts an existing AnimatorOverrideController to prevent creating new instances on every call,
    // /// improving performance and memory management.
    // /// </summary>
    // public static void Play_EO_IDLE_Animation(Animator animator, AnimatorOverrideController overrideController, AnimationClip clip)
    // {
    //     if (animator == null || overrideController == null)
    //     {
    //         Debug.LogError("Play_EO_IDLE_Animation: Animator or OverrideController is null.");
    //         return;
    //     }

    //     if (clip == null)
    //     {
    //         Debug.LogWarning($"Play_EO_IDLE_Animation: AnimationClip is null for animator {animator.name}. Using placeholder.");
    //         clip = UtilityLibrary.Instance.nullPlaceholderClip;
    //     }

    //     // Ensure the animator is using the provided override controller.
    //     if (animator.runtimeAnimatorController != overrideController)
    //     {
    //         animator.runtimeAnimatorController = overrideController;
    //     }

    //     // Override the first clip. Assumes the base controller's first clip is the one to replace.
    //     if (overrideController.animationClips.Length > 0)
    //     {
    //         overrideController[overrideController.animationClips[0].name] = clip;
    //     }
    //     else
    //     {
    //         Debug.LogError($"Animator on {animator.name} has no clips in its controller to override.");
    //         return;
    //     }

    //     // SETUP
    //     animator.SetFloat("Speed", 1);
    //     animator.Play("BaseState", 0, 0f);
    // }

    // /// <summary>
    // /// Overrides a clip and plays it for a specific duration by adjusting the animator's speed.
    // /// EDITED: Now accepts an existing AnimatorOverrideController to improve performance.
    // /// </summary>
    // public static void Play_EO_IDLE_AnimationForDuration(Animator animator, AnimatorOverrideController overrideController, AnimationClip clip, float duration)
    // {
    //     if (animator == null || overrideController == null)
    //     {
    //         Debug.LogError("Play_EO_IDLE_AnimationForDuration: Animator or OverrideController is null.");
    //         return;
    //     }
    //     if (clip == null)
    //     {
    //         Debug.LogWarning($"Play_EO_IDLE_AnimationForDuration: AnimationClip is null for animator {animator.name}. Using placeholder.");
    //         clip = UtilityLibrary.Instance.nullPlaceholderClip;
    //     }
    //     if (duration <= 0f)
    //     {
    //         Debug.LogError("Play_EO_IDLE_AnimationForDuration: Duration must be greater than zero.");
    //         return;
    //     }

    //     // Ensure the animator is using the provided override controller.
    //     if (animator.runtimeAnimatorController != overrideController)
    //     {
    //         animator.runtimeAnimatorController = overrideController;
    //     }

    //     if (overrideController.animationClips.Length > 0)
    //     {
    //         overrideController[overrideController.animationClips[0].name] = clip;
    //     }
    //     else
    //     {
    //         Debug.LogError($"Animator on {animator.name} has no clips in its controller to override.");
    //         return;
    //     }

    //     // SETUP
    //     float playbackSpeed = (clip.length > 0) ? clip.length / duration : 1f;
    //     animator.SetFloat("Speed", playbackSpeed);
    //     animator.Play("BaseState", 0, 0f);
    // }

    // /// <summary>
    // /// Overrides a clip in an Animator's override controller and plays it, synchronizing the start time
    // /// with the previously playing animation's normalized time to ensure smooth transitions between states like running directions.
    // /// </summary>
    // public static void PlaySyncedWithOverride(Animator animator, AnimatorOverrideController overrideController, string placeholderClipName, AnimationClip newClip, float speed = 1f)
    // {
    //     if (animator == null || overrideController == null || string.IsNullOrEmpty(placeholderClipName) || newClip == null)
    //     {
    //         Debug.LogWarning("PlaySyncedWithOverride called with invalid arguments.");
    //         return;
    //     }

    //     // Capture the normalized time of the current state before changing anything.
    //     float normalizedTime = 0f;
    //     if (animator.runtimeAnimatorController != null && animator.GetCurrentAnimatorStateInfo(0).length > 0)
    //     {
    //         normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
    //         if (normalizedTime < 0) normalizedTime += 1; // Ensure positive value for time
    //     }

    //     // Apply the override and set speed.
    //     overrideController[placeholderClipName] = newClip;
    //     animator.SetFloat("Speed", speed);

    //     // We play the "BaseState", which is where the overridden clip is, from the captured time.
    //     animator.Play("BaseState", 0, normalizedTime);
    // }
}


