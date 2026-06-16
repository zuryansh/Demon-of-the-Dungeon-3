using System.Collections.Generic;

using UnityEngine;

public class EffectContext
{
    GameObject source;
    GameObject target;
    Vector3 effectPoint;
    Vector3 effectDir;


    public EffectContext(GameObject source,GameObject target, Vector3 effectPoint, Vector3 effectDir)
    {
        this.source = source;
        this.target = target;
        this.effectPoint = effectPoint;
        this.effectDir = effectDir;
    }

    public GameObject Source { get => source; }
    public GameObject Target { get => target; }
    public Vector3 EffectPoint { get => effectPoint; }
    public Vector3 EffectDir { get => effectDir; }
}
