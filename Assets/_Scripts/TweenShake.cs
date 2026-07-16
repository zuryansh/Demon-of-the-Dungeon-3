using DG.Tweening;
using EditorAttributes;
using UnityEngine;

public class TweenShake : MonoBehaviour
{
    [SerializeField] Transform applyTo;
    [SerializeField] float duration;

    [SerializeField] private Transform target;

    private Tween shakeTween;

    private void Awake()
    {
        if (target == null)
            target = transform;
    }

    [Button("Shake")]
    public void Shake(float duration = 0.2f, float strength = 12f, int vibrato = 20, float randomness = 90f)
    {
        shakeTween?.Kill();

        shakeTween = target.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness,
            fadeOut: true
        );
    }
}

