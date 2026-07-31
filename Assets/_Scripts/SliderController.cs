using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] float displayTime;
    [SerializeField] CanvasGroup fader;
    [SerializeField] float fadeDuration;
    //[SerializeField] bool isPlayerHealthBar;
    [SerializeField] Health attatchedHealth;
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
        if (attatchedHealth != null) attatchedHealth.EOnHealthChange.RemoveListener(UpdateSliderVal);
    }

    public void RegisterHealthBarUser(Health health)
    {
        if (health == null) return;
        health.EOnHealthChange.AddListener(UpdateSliderVal);
        attatchedHealth = health;
    }


    private void OnDestroy()
    {
        if (attatchedHealth != null) attatchedHealth.EOnHealthChange.RemoveListener(UpdateSliderVal);
            
        
        if (fader != null) DOTween.Kill(fader);
    }

}
