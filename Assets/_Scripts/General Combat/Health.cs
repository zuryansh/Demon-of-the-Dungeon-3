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


        if(curHealth <= 0 )
        {
            OnDeath.Invoke(cntxt);
        }
    }

    

}
