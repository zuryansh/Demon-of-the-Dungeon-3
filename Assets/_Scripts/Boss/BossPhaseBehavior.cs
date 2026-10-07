using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class BossPhaseBehavior
{
    public abstract IEnumerator Execute(Boss boss);
}

public class ChargeAttackBehavior : BossPhaseBehavior
{
    public override IEnumerator Execute(Boss boss)
    {
        Debug.Log("CHARGE BEHAVIOR EXECUTE");
        yield break;
    }
}