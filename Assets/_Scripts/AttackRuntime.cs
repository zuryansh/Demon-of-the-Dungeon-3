using System;
using UnityEngine;

[System.Serializable]
public class AttackRuntime
{

    public AttackData Data => data;

    [SerializeField] AttackData data;
    [SerializeField] float progress=0f;
    float startTime;
    Animator animator;
    float prevProgress = 0f;

    public event Action<bool> EToggleMouseLock;
    public event Action EAttackFinish;

    public AttackRuntime(AttackData data, float startTime, Animator animator) 
    {
        this.data = data;
        this.startTime = startTime;
        this.animator = animator;
        if (Data == null) Debug.LogWarning("ASDSA");
        progress = 0f;
        prevProgress = 0f;
    }

    public void Tick()
    {
        prevProgress = progress;
        progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if(data.MouseLockTime >= 0)
        {
            if(Crossed(prevProgress, progress, data.MouseLockTime))
            {
                EToggleMouseLock?.Invoke(true); //lock mouse
            }
        }

        if(Crossed(prevProgress,progress, 1f))
        {
            
            AttackFinish();
            EAttackFinish?.Invoke();
            Dispose();
        }
    }


    void AttackFinish()
    {
        EToggleMouseLock?.Invoke(false); //unlock mouse
    }

    void Dispose() { EAttackFinish = null; EToggleMouseLock = null; }
    public bool CanBufferNextAttack()
    {
        return progress >= data.NextAttackInputStartTime;
    }

    //bool InWindow(float val, float start, float end) => (val >= start && val <= end);

    bool Crossed(float prev, float next, float threshold)
    {
        return (prev < threshold && next >= threshold);
    }
}
