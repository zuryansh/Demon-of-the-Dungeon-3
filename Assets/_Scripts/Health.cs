using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public UnityEvent<EffectContext> OnDamage;
    public UnityEvent OnDeath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(EffectContext cntxt,float dmg)
    {
        curHealth -= dmg;
        OnDamage.Invoke(cntxt);
        if(curHealth < 0 )
        {
            OnDeath.Invoke();
        }
    }
}
