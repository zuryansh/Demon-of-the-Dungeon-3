using System;
using TMPro;
using UnityEngine;



public class PlayerCounterText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] TweenShake shaker;
    [SerializeField] ParticleSystem particles;

    private void Start()
    {
        Player.Instance.EOnPointsChanged += UpdateCounter;
    }

    void UpdateCounter(float n)
    {
        counterText.text = n.ToString();
        if(shaker!=null) shaker.Shake();
        if(particles != null) particles.Play();
    }
}
