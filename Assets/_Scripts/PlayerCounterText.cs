using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCounterText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] TweenShake shaker;
    [SerializeField] ParticleSystem particles;
    UnityEvent<float> registeredPointsChangedEvent;


    public void RegisterCounterTo(UnityEvent<float> updateEvent)
    {
        updateEvent.AddListener(UpdateCounter);
        registeredPointsChangedEvent = updateEvent;
    }

    void UpdateCounter(float n)
    {
        counterText.text = n.ToString();
        if(shaker!=null) shaker.Shake();
        if(particles != null) particles.Play();
    }

    private void OnDisable()
    {
        if (registeredPointsChangedEvent != null) registeredPointsChangedEvent.RemoveListener(UpdateCounter);

    }
}
