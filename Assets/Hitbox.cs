using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{

    public event Action<Collider2D> EOnHitDetect;

    [SerializeField]Collider2D col;
    [SerializeField] HashSet<Collider2D> detectedColliders = new HashSet<Collider2D>();
    [SerializeField] LayerMask layerMask;

    ContactFilter2D contactFilter;
    List<Collider2D> temp = new List<Collider2D>();

    void Start()
    {
        if (col == null) col = GetComponent<Collider2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = layerMask;
        contactFilter.useLayerMask= true;
    }

    void FixedUpdate()
    {
        temp.Clear();

        if (Physics2D.OverlapCollider(col, contactFilter, temp) == 0)
            return;

        foreach (var collider in temp)
        {
            if (detectedColliders.Add(collider))
            {
                EOnHitDetect?.Invoke(collider);
            }
        }
    }


    private void OnEnable()
    {
        ResetHitbox();
    }

    public void ResetHitbox()
    {
        detectedColliders.Clear();
    }

}
