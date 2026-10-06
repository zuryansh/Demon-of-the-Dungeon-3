using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopupText : MonoBehaviour
{

    [SerializeField] CanvasGroup canvas;
    [SerializeField] TextMeshProUGUI text;

    Sequence sequence;
    PopupTextManager popupTextManager;

    public void Initialize(PopupTextManager manager)
    {
        this.popupTextManager = manager;
    }

    public void Show(string text, Vector3 position, Color color,float scale=1, float fadeDuration= 0.5f,float activeDuration =1f)
    {
        transform.position = position;
        this.text.text = text;
        this.text.color = color;
        canvas.alpha = 1f;
        canvas.transform.localScale = Vector3.one * scale;
        Animate(scale,activeDuration, fadeDuration);
    }

    private void Animate(
    float scale,
    float activeDuration,
    float fadeDuration)
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(0.3f, 0.6f);
        float moveDuration = Random.Range(0.15f, 0.25f);

        Vector3 targetPosition =
            transform.position + (Vector3)(direction * distance);

        Vector3 targetScale = Vector3.one * scale;

        canvas.transform.localScale = targetScale * 0.5f;

        sequence = DOTween.Sequence()
            .Append(
                canvas.transform
                    .DOScale(targetScale, moveDuration)
                    .SetEase(Ease.OutElastic)
            )
            .Join(
                transform.DOMove(targetPosition, moveDuration)
                    .SetEase(Ease.OutQuad)
            )
            .AppendInterval(activeDuration)
            .Append(
                canvas.DOFade(0f, fadeDuration)
            )
            .OnComplete(() => popupTextManager.Release(this));
    }
    public void ResetState()
    {
        sequence?.Kill();
        sequence = null;
        text.color = Color.white;
        canvas.alpha = 1f;
        canvas.transform.localScale = Vector3.one;
    }
}




