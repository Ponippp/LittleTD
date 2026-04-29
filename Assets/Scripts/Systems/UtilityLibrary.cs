// Assets/Scripts/Systems/UtilityLibrary.cs
using UnityEngine;

/// <summary>
/// Scene singleton that holds global utility assets.
/// Attach to a persistent GameObject in the scene (e.g. "SystemManager").
/// </summary>
public class UtilityLibrary : MonoBehaviour //has to be on an object in unity to exist b/c of monobehavior, makes it easier to put nullPlaceHolderClip in the editor instead of in code
{
    public static UtilityLibrary Instance { get; private set; }

    [Tooltip("Fallback AnimationClip used when a real clip cannot be found. " +
             "Assign the same placeholder clip that lives in EnemyAnimatorController's BaseState.")]
    public AnimationClip nullPlaceholderClip; //debugging red arrow

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
