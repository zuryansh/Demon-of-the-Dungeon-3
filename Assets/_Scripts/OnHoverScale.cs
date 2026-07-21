using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class OnHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float highlighDuration;
    [SerializeField] Ease highlightEase;
    [SerializeField] Transform applyTo;
    [SerializeField] AudioClip[] onHoverSounds;
    [SerializeField] float volume;

    Vector3 ogScale;

    void Start()
    {
        ogScale = applyTo.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverSounds.Length != 0) AudioManager.Instance.PlayRandomSound(onHoverSounds, volume, SoundType.Sfx);
        applyTo.DOScale(applyTo.localScale + (Vector3.one * 0.1f), highlighDuration).SetEase(highlightEase).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        applyTo.DOScale(ogScale, highlighDuration).SetEase(highlightEase).SetUpdate(true);

    }

    public void Bounce()
    {
        applyTo.DOShakeScale(0.2f);
    }

    void OnDestroy()
    {
        applyTo.DOKill();
    }


}