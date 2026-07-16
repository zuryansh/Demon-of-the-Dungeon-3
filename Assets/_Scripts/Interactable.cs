using EditorAttributes;
using System;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.IsInLayerMask(interactableLayer)) 
            Interact(collision.gameObject);
    }

    protected virtual void Interact(GameObject interactor)
    {
        if (interactableOnce && interacted) return;
        interacted = true;
        EffectContext c = new EffectContext(gameObject, interactor, transform.position, transform.up);
        foreach (Effect effect in onInteractEffects)
        {
            effect.Apply(c);
        }
        if (HasAnimation) animHelper.ChangeAnimation(Animator.StringToHash(interactClip.name));
        EOnInteract?.Invoke();
    }

}
