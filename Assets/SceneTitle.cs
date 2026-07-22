using TMPro;
using UnityEngine;

public class SceneTitle : MonoBehaviour
{
    [SerializeField] AnimationClip animationClip;
    [SerializeField] string title;
    [SerializeField] TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnimationHelper helper = GetComponent<AnimationHelper>();
        text.text = title;
        helper.ChangeAnimation(Animator.StringToHash(animationClip.name));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
