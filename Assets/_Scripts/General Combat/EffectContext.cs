using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class EffectContext
{
    protected GameObject source;
    protected GameObject target;
    protected Vector3 effectPoint;
    protected Vector3 effectDir;
    protected Vector3 targetPos;


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
    public  virtual GameObject Source { get => source; }
    public  virtual GameObject Target { get => target; }
    public  virtual Vector3 EffectPoint { get => effectPoint; }
    public virtual  Vector3 EffectDir { get => effectDir; }
}

//public class BuffDebuffContext : EffectContext
//{
//    public BuffDebuffContext(GameObject source, GameObject target, Vector3 effectPoint, Vector3 effectDir) : base(source, target, effectPoint, effectDir)
//    {

//    }
//}