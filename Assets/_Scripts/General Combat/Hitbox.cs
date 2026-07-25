using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{

    public event Action<Collider2D,Vector3> EOnHitDetect;
    public event Action OnHitboxDisable;

    [SerializeField]Collider2D col;
    [SerializeField] HashSet<Collider2D> detectedColliders = new HashSet<Collider2D>();
    [SerializeField] LayerMask layerMask;

    ContactFilter2D contactFilter;
    List<RaycastHit2D> temp1 = new List<RaycastHit2D>();
    List<Collider2D> temp2 = new List<Collider2D>();
    Vector2 prevPos;


    void Start()
    {
        if (col == null) col = GetComponent<Collider2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = layerMask;
        contactFilter.useLayerMask= true;
        prevPos = transform.position;
    }

    void Update()
    {

        temp1.Clear();
        temp2.Clear();

        Vector2 currentPosition = transform.position;
        Vector2 delta = currentPosition - prevPos;

        if (delta.sqrMagnitude > Mathf.Epsilon)
        {
            col.Cast(delta.normalized, contactFilter, temp1, delta.magnitude + 0.01f);

            foreach (var hit in temp1)
            {
                if (detectedColliders.Add(hit.collider))
                {
                    if (hit.transform != null)
                    {
                        Vector3 dir = (hit.transform.position - transform.position).normalized;
                        EOnHitDetect?.Invoke(hit.collider, dir);
                    }
                }
            }
        }

        if (Physics2D.OverlapCollider(col, contactFilter, temp2) > 0)
        {


            foreach (var collider in temp2)
            {
                if (detectedColliders.Add(collider))
                {
                    Vector3 dir = (collider.transform.position - col.transform.position).normalized;
                    EOnHitDetect?.Invoke(collider, dir);
                }
            }
        }

        prevPos = currentPosition;
    }

    Vector3 GetHitDirection()
    {
        throw new NotImplementedException();
    }

    private void OnEnable()
    {
        ResetHitbox();
    }

    private void OnDisable()
    {
        OnHitboxDisable?.Invoke(); // for when we want to use the whole list of colliders at once
    }

    public void ResetHitbox()
    {
        detectedColliders.Clear();
    }

}
