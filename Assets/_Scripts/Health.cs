using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    
    [SerializeField] AudioClip[] onHitSound;
    [SerializeField] AudioClip[] onDeathSound;
    public float maxHealth;
    public float curHealth;
    public UnityEvent<EffectContext> OnHit;
    public UnityEvent OnDeath;
    public UnityEvent<float, float> OnHealthChangeUnityEvent;
    public event Action<float, float> EOnHealthChange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(EffectContext cntxt,float dmg)
    {
        curHealth -= dmg;
        OnHit.Invoke(cntxt);
        OnHealthChangeUnityEvent.Invoke(curHealth, maxHealth);
        EOnHealthChange?.Invoke(curHealth, maxHealth);

        if(onHitSound.Length!=0)AudioManager.Instance.PlayRandomSound(onHitSound, 1f, SoundType.Sfx);

        if(curHealth <= 0 )
        {
            if(onDeathSound.Length != 0)AudioManager.Instance.PlayRandomSound(onDeathSound, 1f, SoundType.Sfx);
            OnDeath.Invoke();
        }
    }

    
}
