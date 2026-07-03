using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Hitbox))]
public class Projectile : MonoBehaviour
{
    [SerializeReference, SubclassSelector] List<Effect> onHitEffects;
    [SerializeField] int pierceCount;
    [SerializeField] int projHit;
    
    Hitbox hitbox;


    private void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {
        projHit++;

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in onHitEffects)
            effect.Apply(context);
        if(projHit >= pierceCount) Destroy(gameObject);
    }

    private void OnEnable()
    {
        hitbox.EOnHitDetect += NotifyHit;
    }
    private void OnDisable()
    {
        hitbox.EOnHitDetect -= NotifyHit;
    }


}
