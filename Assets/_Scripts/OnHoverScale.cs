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
    [SerializeField] Vector3 offset = Vector3.one * 0.1f;

    Vector3 ogScale;

    void Start()
    {
        ogScale = applyTo.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverSounds.Length != 0) AudioManager.Instance.PlayRandomSound(onHoverSounds, volume, SoundType.Sfx);
        applyTo.DOScale(applyTo.localScale + offset, highlighDuration).SetEase(highlightEase).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        applyTo.DOScale(ogScale, highlighDuration).SetEase(highlightEase).SetUpdate(true);

    }

    public void Bounce()
    {
        applyTo.DOShakeScale(0.2f, strength:0.5f);
    }

    void OnDestroy()
    {
        applyTo.DOKill();
    }


}