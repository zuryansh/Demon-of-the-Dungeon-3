using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class EffectContext
{
    GameObject source;
    GameObject target;
    Vector3 effectPoint;
    Vector3 effectDir;
    Vector3 targetPos;


    public EffectContext(GameObject source,GameObject target, Vector3 effectPoint, Vector3 effectDir)
    {
        this.source = source;
        this.target = target;
        this.effectPoint = effectPoint;
        this.effectDir = effectDir;

    }

    public EffectContext(GameObject source, Vector3 targetPos, Vector3 effectPoint, Vector3 effDir)
    {
        this.source = source;
        this.targetPos = targetPos;
        this.effectPoint = effectPoint;
        this.effectDir = effDir;
    }

    public Vector3 TargetPos { get
        {
            if (Target == null) return targetPos;
            else return Target.transform.position;
        } }
    public GameObject Source { get => source; }
    public GameObject Target { get => target; }
    public Vector3 EffectPoint { get => effectPoint; }
    public Vector3 EffectDir { get => effectDir; }
}

//public class SpawnEffectContext : EffectContext
//{
//    public ISpawner Spawner => spawner;

//    ISpawner spawner;

//    public SpawnEffectContext(GameObject source,ISpawner parentSpawner, GameObject target, Vector3 effectPoint, Vector3 effectDir) : base(source, target, effectPoint, effectDir)
//    {
//        spawner = parentSpawner;

//    }
//}