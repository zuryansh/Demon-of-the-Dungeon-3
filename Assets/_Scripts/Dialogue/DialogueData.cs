using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(5,10)]
    [SerializeField] string[] paras;


    public string[] Paras => paras;
}


