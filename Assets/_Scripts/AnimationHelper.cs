using UnityEngine;

[RequireComponent (typeof(Animator))]
public class AnimationHelper : MonoBehaviour
{
    public int CurrentAnim => currentAnim;
    public Animator Anim => anim;

    [SerializeField] Animator anim;
    [SerializeField] int currentAnim;
    [SerializeField] int currentAnimPriority;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(anim == null) anim = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, bool forceReplay = false, int priority =0)
    {
        
        if (state == currentAnim && !forceReplay) return;
        if (priority >= currentAnimPriority || anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
        {

            currentAnim = state;
            currentAnimPriority = priority;
            anim.Play(state, -1, 0f);
        }
    }
}
