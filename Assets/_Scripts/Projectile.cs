using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float activeTime =5f;
    [SerializeReference, SubclassSelector] protected List<Effect> onHitEffects;
    [SerializeField] protected int pierceCount;
    [SerializeField] protected int projHit;
    [SerializeField] Hitbox hitbox;
    
    public Transform Sender;
    protected Rigidbody2D rb;

    private void Awake()
    {
        if(hitbox == null )hitbox = GetComponent<Hitbox>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, activeTime);
    }

    virtual public  void Launch(Vector3 vel)
    {
        rb.linearVelocity = vel;
    }

    public virtual void NotifyHit(Collider2D collider, Vector3 dir)
    {
        Debug.Log("HIUT");

        projHit++;

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in onHitEffects)
            effect.Apply(context);
        if (projHit >= pierceCount) OnPierceFinish();
    }

    protected virtual void OnPierceFinish()
    {
        Destroy(gameObject);
    }

   protected virtual void OnEnable()
    {
        hitbox.EOnHitDetect += NotifyHit;
    }
   protected virtual void OnDisable()
    {
        hitbox.EOnHitDetect -= NotifyHit;
    }


}
