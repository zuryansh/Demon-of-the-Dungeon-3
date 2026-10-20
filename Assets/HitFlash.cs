using UnityEngine;
using DG.Tweening;
using EditorAttributes;

public class HitFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Ease ease;
    float currentFlashAmount;
    [SerializeField]float flashTime = 0.3f;
    [SerializeField, Range(0f,1f)] float flashAmount=1f;

    Tween flashTween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteRenderer.material.GetFloat("_FlashAmount") != currentFlashAmount) spriteRenderer.material.SetFloat("_FlashAmount", currentFlashAmount);
    }

    [Button("Flash")]
    public void Flash()
    {
        flashTween?.Kill();
        currentFlashAmount = flashAmount;
        flashTween = DOTween.To(() => currentFlashAmount, x => currentFlashAmount= x, 0, flashTime).SetEase(ease);
    }
    void OnDestroy()
    {
        flashTween?.Kill();
    }
}
