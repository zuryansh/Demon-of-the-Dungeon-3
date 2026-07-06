using System;
using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class Teleporter : MonoBehaviour
{

    public CircleCollider2D Collider => col;
    
    [SerializeField] protected Vector2 teleportTo;
    [SerializeField] protected CircleCollider2D col;
    [SerializeField] protected bool locked;


    protected virtual void Teleport(GameObject obj, string tag = "")
    {
        obj.transform.position = teleportTo;
        print($"Teleported: {obj.name}");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
       if(!locked) Teleport(collision.gameObject);
    }

    public void SetLock(bool val) => locked = val;

    public void SetTeleportTo(Vector2 pos) { teleportTo = pos; }
}


