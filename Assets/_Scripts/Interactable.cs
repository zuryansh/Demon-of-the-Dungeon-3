using EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public event Action EOnInteract;
    bool HasAnimation => animHelper!=null;

    [SerializeField] protected LayerMask interactableLayer;
    [SerializeField] bool interactableOnce;
    [SerializeField] AnimationHelper animHelper;
    [SerializeField, ShowField(nameof(interactableOnce))] bool interacted;
    [SerializeField, ShowField(nameof(HasAnimation))] AnimationClip interactClip;
    [SerializeReference, SubclassSelector] protected List<Effect> onInteractEffects;
    [SerializeField] float interactCooldown = 0.6f;

    bool canInteract = true;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.IsInLayerMask(interactableLayer) && canInteract)
        {
            if(collision.TryGetComponent(out Interactor interactor))
            {

                interactor.OnInteractableClose(this);
            }
        }
            //Interact(collision.gameObject);
    }

    public virtual bool Interact(GameObject interactor)
    {
        if (interactableOnce && interacted || !canInteract) return false;
        print("WE INTERACTED");
        interacted = true;
        canInteract = false;

        EffectContext c = new EffectContext(gameObject, interactor, transform.position, transform.right);
        foreach (Effect effect in onInteractEffects)
        {
            effect.Apply(c);
        }

        if (HasAnimation) animHelper.ChangeAnimation(Animator.StringToHash(interactClip.name));
        EOnInteract?.Invoke();
        StartCoroutine(OnInteractFinish());
        return true;
    }

    public virtual IEnumerator OnInteractFinish()
    {
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }

}
