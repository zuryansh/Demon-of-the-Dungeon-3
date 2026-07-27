using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public Transform Sender;
    [SerializeField] float activeTime =5f;
    [SerializeReference, SubclassSelector] protected List<Effect> onHitEffects;
    [SerializeReference, SubclassSelector] protected List<Effect> onLaunchEffects;
    [SerializeField] protected int pierceCount;
    [SerializeField] protected int projHit;
    [SerializeField] protected Hitbox hitbox;
    [SerializeField] protected LayerMask wallLayer;


    protected Rigidbody2D rb;
    protected bool pierceFinished = false;

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
        Vector3 p = transform.position;

        EffectContext context = new EffectContext(gameObject, null, p, vel.normalized);

        foreach (Effect effect in onLaunchEffects)
            effect.Apply(context);
    }

    public virtual void NotifyHit(Collider2D collider, Vector3 dir)
    {
       
        if (collider.gameObject.IsInLayerMask(wallLayer)) { OnPierceFinish(); return; }
        if(Sender!= null) if (collider.gameObject == Sender.gameObject) return;

        projHit++;

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in onHitEffects)
            effect.Apply(context);
        if (projHit >= pierceCount) OnPierceFinish();
    }

    protected virtual void OnPierceFinish()
    {
        pierceFinished = true;
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
