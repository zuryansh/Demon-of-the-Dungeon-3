using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Hitbox))]
public class Projectile : MonoBehaviour
{
    [SerializeReference, SubclassSelector] List<Effect> onHitEffects;
    Hitbox hitbox;


    private void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    public void NotifyHit(Collider2D collider, Vector3 dir)
    {

        Vector3 p = collider.ClosestPoint(transform.position);

        EffectContext context = new EffectContext(gameObject, collider.gameObject, p, dir);

        foreach (Effect effect in onHitEffects)
            effect.Apply(context);
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
