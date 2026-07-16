using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PulseTween : MonoBehaviour
{
    [SerializeField] float pulseDuration;
    [SerializeField] float factor;
    [SerializeField] Ease highlightEase;
    [SerializeField] Transform applyTo;

    Vector3 ogScale;

    void Start()
    {
        ogScale = transform.localScale;
        Pulse();
    }

    public void Pulse()
    {
        applyTo.DOScale(transform.localScale *factor, pulseDuration).SetEase(highlightEase).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
    }


    void OnDestroy()
    {
        applyTo.DOKill();
    }
}
