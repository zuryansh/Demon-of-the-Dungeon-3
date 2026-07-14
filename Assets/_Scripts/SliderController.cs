using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] float displayTime;
    [SerializeField] CanvasGroup fader;
    [SerializeField] float fadeDuration;
    [SerializeField] bool isPlayerHealthBar;
    Slider slider;


    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (displayTime > 0f) Hide();
    }

    public void UpdateSliderVal(float val, float maxVal)
    {
        Show();
        slider.value = val/maxVal;
        if(displayTime > 0) Invoke(nameof(Hide), displayTime);
    }

    void Show()
    {
        if (fader != null) fader.alpha = 1f;
    }

    void Hide()
    {
        if (fader != null) fader.DOFade(0f, fadeDuration);
    }


    private void OnEnable()
    {
        if (isPlayerHealthBar)
        {
            if (Player.Instance.PlayerHealth == null)
            {
                Debug.LogError("Player Health Bar Exists without Player");

            }
            else Player.Instance.PlayerHealth.EOnHealthChange.AddListener(UpdateSliderVal);
        }
    }

    private void OnDestroy()
    {
        if (isPlayerHealthBar)
        {
            if (Player.Instance != null)
            {
                if (Player.Instance.PlayerHealth == null)
                {
                    Debug.LogError("Player Health Bar Exists without Player");

                }
                else Player.Instance.PlayerHealth.EOnHealthChange.RemoveListener(UpdateSliderVal);
            }
        }
        if (fader != null) DOTween.Kill(fader);
    }

}
