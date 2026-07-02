using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopupText : MonoBehaviour
{

    [SerializeField] CanvasGroup canvas;
    [SerializeField] TextMeshProUGUI text;

    float scale=1f;
    float fadeDuration=0f;




    public void Init(string text,float scale, float activeDuration ,bool doFade, float fadeDuration,bool doAnimation = false)
    {
        this.text.text = text;
        this.scale = scale;
        this.fadeDuration = fadeDuration;
        canvas.transform.localScale = Vector3.one * scale;

        if (doFade)
        {
            Invoke(nameof(FadeOut), activeDuration);
        }
    }

    void FadeOut()
    {
        canvas.DOFade(0f, fadeDuration);
        Destroy(gameObject, fadeDuration);
    }

    private void OnDestroy()
    {
        DOTween.Kill(canvas);
    }
}


