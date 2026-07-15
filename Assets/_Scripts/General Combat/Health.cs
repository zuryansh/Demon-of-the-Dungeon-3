using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    
    public UnityEvent<EffectContext> OnHit;
    public UnityEvent<EffectContext> OnDeath;
    public UnityEvent<float, float> EOnHealthChange;

    [SerializeField] float maxHealth;
    [SerializeField] float curHealth;
    [SerializeField] float invincibilityTime;

    [SerializeField] AudioClip[] onHitSound;
    [SerializeField] AudioClip[] onDeathSound;

    float timeSinceLastHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
    }

    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;
    }

    public void TakeDamage(EffectContext cntxt,float dmg)
    {
        if (timeSinceLastHit < invincibilityTime) return;

        timeSinceLastHit = 0;
        curHealth -= dmg;
        OnHit.Invoke(cntxt);
        EOnHealthChange.Invoke(curHealth, maxHealth);

        if(onHitSound.Length!=0)AudioManager.Instance.PlayRandomSound(onHitSound, 1f, SoundType.Sfx);

        if(curHealth <= 0 )
        {
            if(onDeathSound.Length != 0)AudioManager.Instance.PlayRandomSound(onDeathSound, 1f, SoundType.Sfx);
            OnDeath.Invoke(cntxt);
        }
    }

    
}
