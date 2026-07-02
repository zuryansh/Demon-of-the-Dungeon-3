using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public UnityEvent<EffectContext> OnHit;
    public UnityEvent OnDeath;
    public UnityEvent<float, float> OnHealthChange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(EffectContext cntxt,float dmg)
    {
        curHealth -= dmg;
        OnHit.Invoke(cntxt);
        OnHealthChange.Invoke(curHealth, maxHealth);
        if(curHealth <= 0 )
        {
            OnDeath.Invoke();
        }
    }

    
}
