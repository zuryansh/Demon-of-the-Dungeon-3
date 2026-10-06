using EditorAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;


public class Health : MonoBehaviour
{
    
    public UnityEvent<EffectContext> OnHit;
    public UnityEvent<EffectContext> OnDeath;
    public UnityEvent<float, float> EOnHealthChange;
    public float CurHealth => curHealth;
    public float MaxHealth => maxHealth;
    public bool Vulnerable => vulnerable && (timeSinceLastHit > invincibilityTime);

    [SerializeField] float maxHealth;
    [SerializeField] float curHealth;
    [SerializeField] float invincibilityTime;
    [SerializeField] bool dead;
    [SerializeField] bool spawnDamageText = true;
    [SerializeField, EnableField(nameof(spawnDamageText))] float textScale = 1f;
    [SerializeField, EnableField(nameof(spawnDamageText))] private Gradient damagePopupGradient;
    float timeSinceLastHit;
    [SerializeField]bool vulnerable = true;

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
        if (!vulnerable) return;

        timeSinceLastHit = 0;
        curHealth -= dmg;
        if (spawnDamageText)
        {
            float damageRatio = Mathf.Clamp01(dmg / maxHealth);
            Color popupColor = damagePopupGradient.Evaluate(damageRatio);
            PopupTextManager.Instance.Show(((int)dmg).ToString(), cntxt.EffectPoint, popupColor,scale: textScale, fadeDuration: 0.2f);

        }

        OnHit.Invoke(cntxt);
        EOnHealthChange.Invoke(curHealth, maxHealth);


        if(curHealth <= 0 && !dead)
        {
            OnDeath.Invoke(cntxt);
            dead = true;
        }
    }

    public void SetVulnerability(bool val)
    {
        vulnerable = val;
    }

}
