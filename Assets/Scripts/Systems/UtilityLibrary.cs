// Assets/Scripts/Systems/UtilityLibrary.cs
using UnityEngine;

/// <summary>
/// Scene singleton that holds global utility assets.
/// Attach to a persistent GameObject in the scene (e.g. "SystemManager").
/// </summary>
public class UtilityLibrary : MonoBehaviour
{
    public static UtilityLibrary Instance { get; private set; }

    [Tooltip("Fallback AnimationClip used when a real clip cannot be found. " +
             "Assign the same placeholder clip that lives in EnemyAnimatorController's BaseState.")]
    public AnimationClip nullPlaceholderClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
