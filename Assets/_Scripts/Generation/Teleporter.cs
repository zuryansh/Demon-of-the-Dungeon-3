using System;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class Teleporter : Interactable
{
    public Vector2 TeleportTo=> teleportTo;
    public CircleCollider2D Collider => col;
    
    [SerializeField] protected Vector2 teleportTo;
    [SerializeField] protected CircleCollider2D col;
    [SerializeField] protected bool locked;
    [SerializeField] protected float delay =0f;


    protected virtual void Teleport(GameObject obj, string tag = "")
    {
        obj.transform.position = teleportTo;
    }



    public void SetLock(bool val) => locked = val;

    public void SetTeleportTo(Vector2 pos) { teleportTo = pos; }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        //Teleport(interactor);
        StartCoroutine(StartTeleport(interactor));
    }

    protected IEnumerator StartTeleport(GameObject interactor)
    {
        yield return new WaitForSeconds(delay);
        Teleport(interactor);
    }
}


