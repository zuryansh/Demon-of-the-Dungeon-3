using UnityEngine;

[RequireComponent (typeof(Animator))]
public class AnimationHelper : MonoBehaviour
{
    public int CurrentAnim => currentAnim;
    public Animator Anim => anim;

    [SerializeField] Animator anim;
    [SerializeField] int currentAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(anim == null) anim = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float fadeTime = 0f)
    {

        if (state == currentAnim) return;
        currentAnim = state;
        anim.CrossFade(state, fadeTime);
    }
}
