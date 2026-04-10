// Assets/Scripts/Enemy/EnemyAnimator.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Playback speed as a percentage. 100 = normal, 200 = double speed, 50 = half speed.")]
    [Range(0f, 200f)]
    [SerializeField] private float playbackSpeed = 100f;

    [Tooltip("Base frames per second defined in Aseprite.")]
    [SerializeField] private float baseFps = 12f;

    [Header("Frame Ranges (inclusive, matches Frame_N numbers)")]
    [SerializeField] private int runDownFrom = 1;
    [SerializeField] private int runDownTo = 5;
    [SerializeField] private int runUpFrom = 8;
    [SerializeField] private int runUpTo = 13;
    [SerializeField] private int runRightFrom = 16;
    [SerializeField] private int runRightTo = 21;

    // ── Private state ─────────────────────────────────────────────────────────

    private SpriteRenderer _spriteRenderer;
    private Enemy _enemy;

    private SpriteLoader.EnemyAnimations _anims;
    private bool _animsLoaded = false;

    private EnemyDirection _currentDirection = EnemyDirection.Down;
    private float _frameProgress = 0f;
    private float _frameTimer = 0f;

    private Vector3 _previousPosition;

    private enum EnemyDirection { Up, Down, Right, Left }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemy = GetComponentInParent<Enemy>();
        if (_enemy == null) Debug.LogError("[EnemyAnimator] No Enemy component found.");
    }

    private void Start()
    {
        _previousPosition = transform.position;
        TryLoadAnimations();
    }

    private void Update()
    {
        if (!_animsLoaded)
        {
            TryLoadAnimations();
            return;
        }

        UpdateDirection();
        AdvanceFrame();
        ApplyFrame();
    }

    private void TryLoadAnimations()
    {
        if (SpriteLoader.instance == null) return;

        string enemyName = _enemy.GetName().Replace("(Clone)", "").Trim();

        _anims = SpriteLoader.instance.LoadEnemySprites(
            enemyName,
            downRange: new[] { runDownFrom, runDownTo },
            upRange: new[] { runUpFrom, runUpTo },
            rightRange: new[] { runRightFrom, runRightTo }
        );

        bool hasAny = (_anims.runUp != null && _anims.runUp.Count > 0) || (_anims.runDown != null && _anims.runDown.Count > 0) || (_anims.runRight != null && _anims.runRight.Count > 0);

        if (hasAny) _animsLoaded = true;
    }

    private void UpdateDirection()
    {
        Vector3 delta = transform.position - _previousPosition;
        _previousPosition = transform.position;

        if (delta.sqrMagnitude < 0.00001f) return;

        EnemyDirection newDir = ResolveDirection(delta);

        if (newDir != _currentDirection)
        {
            float progress = GetNormalisedProgress();
            _currentDirection = newDir;
            SetNormalisedProgress(progress);
        }
    }

    private static EnemyDirection ResolveDirection(Vector3 delta)
    {
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            return delta.x >= 0f ? EnemyDirection.Right : EnemyDirection.Left;

        return delta.y >= 0f ? EnemyDirection.Up : EnemyDirection.Down;
    }

    private void AdvanceFrame()
    {
        List<Sprite> sheet = CurrentSheet();
        if (sheet == null || sheet.Count == 0) return;

        float effectiveFps = baseFps * (playbackSpeed / 100f);
        if (effectiveFps <= 0f) return;

        float frameDuration = 1f / effectiveFps;
        _frameTimer += Time.deltaTime;

        while (_frameTimer >= frameDuration)
        {
            _frameTimer -= frameDuration;
            _frameProgress = (_frameProgress + (1f / sheet.Count)) % 1f;
        }
    }

    private void ApplyFrame()
    {
        List<Sprite> sheet = CurrentSheet();
        if (sheet == null || sheet.Count == 0) return;

        int index = Mathf.Clamp(Mathf.FloorToInt(_frameProgress * sheet.Count), 0, sheet.Count - 1);
        _spriteRenderer.sprite = sheet[index];
        _spriteRenderer.flipX = (_currentDirection == EnemyDirection.Left);
    }

    private List<Sprite> CurrentSheet() => _currentDirection switch
    {
        EnemyDirection.Up => _anims.runUp,
        EnemyDirection.Down => _anims.runDown,
        EnemyDirection.Right => _anims.runRight,
        EnemyDirection.Left => _anims.runRight,
        _ => _anims.runDown,
    };

    private float GetNormalisedProgress()
    {
        List<Sprite> sheet = CurrentSheet();
        if (sheet == null || sheet.Count == 0) return 0f;
        return (float)Mathf.FloorToInt(_frameProgress * sheet.Count) / sheet.Count;
    }

    private void SetNormalisedProgress(float t)
    {
        _frameProgress = Mathf.Clamp01(t);
        _frameTimer = 0f;
    }
}