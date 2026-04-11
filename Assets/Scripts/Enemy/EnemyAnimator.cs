// Assets/Scripts/Enemy/EnemyAnimator.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drives enemy locomotion via Animator + duplicated AnimatorOverrideController.
/// Per-enemy clips come from Resources (see SpriteLoader.LoadEnemyRunClips); base controller uses placeholders named Enemy_RUN_DOWN / Enemy_RUN_UP / Enemy_RUN_RIGHT.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    public const string RunStateName = "RUN";
    public const string SpeedParam = "Speed";
    public const string BlendXParam = "BlendX";
    public const string BlendYParam = "BlendY";

    [Header("Animation")]
    [Tooltip("Scales animator Speed parameter. 100 = 1.0 on the animator.")]
    [Range(0f, 200f)]
    [SerializeField] private float playbackSpeed = 100f;

    private static readonly int SpeedHash = Animator.StringToHash(SpeedParam);
    private static readonly int BlendXHash = Animator.StringToHash(BlendXParam);
    private static readonly int BlendYHash = Animator.StringToHash(BlendYParam);

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Enemy _enemy;

    private AnimatorOverrideController _runtimeOverride;
    private bool _initialized;
    private Vector3 _previousPosition;
    private Vector2 _blend = new Vector2(0f, -1f);

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _enemy = GetComponentInParent<Enemy>();
    }

    private void Start()
    {
        _previousPosition = transform.position;
        TryInitializeAnimator();
    }

    private void Update()
    {
        if (!_initialized)
        {
            TryInitializeAnimator();
            return;
        }

        Vector3 delta = transform.position - _previousPosition;
        _previousPosition = transform.position;

        Vector2 velocity = new Vector2(delta.x, delta.y) / Mathf.Max(Time.deltaTime, 1e-5f);
        if (velocity.sqrMagnitude > 0.0001f) _blend = velocity.normalized;

        _animator.SetFloat(BlendXHash, _blend.x);
        _animator.SetFloat(BlendYHash, _blend.y);
        _animator.SetFloat(SpeedHash, ResolveAnimatorSpeed());

        _spriteRenderer.flipX = _blend.x < 0f;
    }

    private float ResolveAnimatorSpeed()
    {
        float pct = _enemy != null && _enemy.GetIsInitialized() ? _enemy.GetAnimationSpeedPercentage() : playbackSpeed;
        return pct / 100f;
    }

    private void TryInitializeAnimator()
    {
        if (_initialized) return;
        if (GameManager.instance == null || SpriteLoader.instance == null) return;

        // EnemyFactory applies EnemyData in Start(); EnemyAnimator Start/Update can run first.
        // Loading clips before Initialize() uses the wrong enemyName and locks _initialized forever.
        if (_enemy != null && !_enemy.GetIsInitialized())
            return;

        AnimatorOverrideController template = GameManager.EnemyAnimatorOverrideTemplate;
        if (template == null)
        {
            Debug.LogError("[EnemyAnimator] GameManager has no AnimatorOverrideController template assigned.");
            return;
        }

        string enemyName = _enemy != null ? _enemy.GetName().Replace("(Clone)", "").Trim() : gameObject.name;

        SpriteLoader.EnemyRunClips clips = SpriteLoader.instance.LoadEnemyRunClips(enemyName);

        _runtimeOverride = Instantiate(template);
        ApplyEnemyOverrides(_runtimeOverride, clips);
        _animator.runtimeAnimatorController = _runtimeOverride;

        _animator.SetFloat(BlendXHash, _blend.x);
        _animator.SetFloat(BlendYHash, _blend.y);
        _animator.SetFloat(SpeedHash, ResolveAnimatorSpeed());
        _animator.Play(RunStateName, 0, 0f);

        _initialized = true;
    }

    private static void ApplyEnemyOverrides(AnimatorOverrideController aoc, SpriteLoader.EnemyRunClips clips)
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        aoc.GetOverrides(overrides);

        var uniqueOriginals = new List<AnimationClip>();
        foreach (KeyValuePair<AnimationClip, AnimationClip> p in overrides)
        {
            if (p.Key != null && !uniqueOriginals.Contains(p.Key))
                uniqueOriginals.Add(p.Key);
        }

        AnimationClip fallback = UtilityLibrary.Instance != null ? UtilityLibrary.Instance.nullPlaceholderClip : null;
        AnimationClip anyEnemy = clips.AnyNonNull();

        var replacementByOriginal = new Dictionary<AnimationClip, AnimationClip>();
        for (int i = 0; i < uniqueOriginals.Count; i++)
        {
            AnimationClip orig = uniqueOriginals[i];
            AnimationClip repl = ResolveOverrideForOriginal(orig.name, clips);

            if (repl == null && uniqueOriginals.Count == 3 && clips.runDown != null && clips.runUp != null && clips.runRight != null)
            {
                repl = i == 0 ? clips.runDown : i == 1 ? clips.runRight : clips.runUp;
            }

            if (repl == null) repl = anyEnemy ?? fallback ?? orig;

            replacementByOriginal[orig] = repl;
        }

        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            if (original == null) continue;

            AnimationClip replacement = replacementByOriginal.TryGetValue(original, out AnimationClip r) ? r : anyEnemy ?? fallback ?? original;
            overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, replacement);
        }

        aoc.ApplyOverrides(overrides);
    }

    private static AnimationClip ResolveOverrideForOriginal(string originalClipName, SpriteLoader.EnemyRunClips clips)
    {
        if (string.IsNullOrEmpty(originalClipName)) return null;

        string u = originalClipName.ToUpperInvariant();
        if (u.Contains("RUN_DOWN") || (u.Contains("DOWN") && u.Contains("RUN"))) return clips.runDown;
        if (u.Contains("RUN_UP")) return clips.runUp;
        if (u.Contains("RUN_RIGHT") || u.Contains("RUN_LEFT")) return clips.runRight;

        return null;
    }
}
