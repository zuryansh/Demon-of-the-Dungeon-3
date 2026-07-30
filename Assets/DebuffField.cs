using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(Collider2D))]
public class DebuffField : MonoBehaviour
{
    [SerializeReference, SubclassSelector] List<BuffDebuffEffect> DebuffEffects;
    [SerializeField] LayerMask layer;
    Collider2D col;
    [SerializeField] float timeBetweenChecks=0.1f;

    List<DebuffManager> effectedObjects;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        
    }

    //IS INEFFICINET TODO: ADD KEEPING A LIST OF COLLIDERS IN AND OUT OF IT;
    private void OnTriggerStay2D(Collider2D collision)
    {

        if(collision.gameObject.IsInLayerMask(layer))
        {
            if(collision.TryGetComponent(out DebuffManager manager))
            {
                Vector3 dir = (collision.transform.position - col.transform.position).normalized;
                EffectContext c = new EffectContext(gameObject, collision.gameObject, collision.ClosestPoint(transform.position), dir);
                foreach (BuffDebuffEffect effect in DebuffEffects)
                {
                        effect.Apply(c);
                }
            }
        }

        
    }

    
}
