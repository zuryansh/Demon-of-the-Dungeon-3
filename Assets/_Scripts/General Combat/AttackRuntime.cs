using System;
using UnityEngine;

[System.Serializable]
public class AttackRuntime
{

    public AttackData Data => data;

    [SerializeField] AttackData data;
    [SerializeField] float progress = 0f;
    float startTime;
    Animator animator;
    float prevProgress = 0f;

    public event Action<bool> EToggleMouseLock;
    public event Action EAttackFinish;

    bool hasTicked = false;

    public AttackRuntime(AttackData data, float startTime, Animator animator)
    {
        this.data = data;
        this.startTime = startTime;
        this.animator = animator;
        if (Data == null) Debug.LogWarning("ASDSA");
        progress = 0f;
        prevProgress = 0f;
        hasTicked = false;
    }

    public void Tick()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Don't trust progress unless we're actually in the attack's own
        // state. We deliberately don't check IsInTransition here: when
        // Play() restarts a state that's already current (duplicate clips
        // back-to-back in a combo), the transition flag can flicker for a
        // tick or two even though nothing meaningful is blending, which
        // would freeze progress at a stale value and never reset it.
        if (stateInfo.shortNameHash != data.AttackAnimation)
            return;

        if (!hasTicked)
        {
            // First real sample of this attack instance. Force a clean
            // baseline instead of trusting whatever normalizedTime happens
            // to read right now, so a fresh AttackRuntime can never inherit
            // a frozen >=1 value left over from a previous instance on the
            // same clip.
            hasTicked = true;
            prevProgress = 0f;
            progress = stateInfo.normalizedTime;
            return;
        }

        prevProgress = progress;
        progress = stateInfo.normalizedTime;

        if (data.MouseLockTime >= 0)
        {
            if (Crossed(prevProgress, progress, data.MouseLockTime))
            {
                EToggleMouseLock?.Invoke(true); //lock mouse
            }
        }

        if (Crossed(prevProgress, progress, 1f))
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