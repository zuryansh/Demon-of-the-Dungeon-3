using UnityEngine;

[RequireComponent (typeof(Animator))]
public class AnimationHelper : MonoBehaviour
{
    public int CurrentAnim => currentAnim;
    public Animator Anim => anim;

    [SerializeField] Animator anim;
    [SerializeField] int currentAnim;
    [SerializeField] int currentAnimPriority;
    [SerializeField] bool debugMode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(anim == null) anim = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float fadeTime = 0f, int priority =0)
    {
        
        if (state == currentAnim) return;
        if (priority >= currentAnimPriority || anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        {
            if (debugMode) Debug.Log(state);

            currentAnim = state;
            currentAnimPriority = priority;
            anim.CrossFade(state, fadeTime);
        }
    }
}
