using DG.Tweening;
using UnityEngine;

public class DoTweenScale : MonoBehaviour
{
    public float duration = 1f;
    public Ease ease = Ease.Linear;
    public Vector3 endPos = Vector3.one;
    public LoopType loopType = LoopType.Restart;
    public int loopCount = 0;
    public bool autoStart = false;

    private Tween _scaleTween;
    private Vector3 _startScale;

    private void Awake()
    {
        _startScale = transform.localScale;
    }

    private void Start()
    {
        if (autoStart)
        {
            StartTween();
        }
    }

    public void StartTween()
    {
        KillTween();
        _scaleTween = transform.DOScale(endPos, duration)
            .SetLoops(loopCount, loopType)
            .SetEase(ease)
            .SetAutoKill(false);
    }

    public void ReverseTween()
    {
        KillTween();
        _scaleTween = transform.DOScale(_startScale, duration)
            .SetLoops(loopCount, loopType)
            .SetEase(ease)
            .SetAutoKill(false);
    }

    public void KillTween()
    {
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }
    }

    private void OnDisable()
    {
        KillTween();
    }

    private void OnDestroy()
    {
        KillTween();
    }
}