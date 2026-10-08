using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using BossArchitecture;
[Serializable]
public class BossPhase
{
    [SerializeField] int phaseID;
    [SerializeField, Range(0, 1)] float phaseEndPoint; 
    [SerializeField] AnimationClip phaseChangeAnim;

    [Header("Behaviors")]
    [SerializeReference, SubclassSelector] List<BossPhaseBehavior> enterBehaviors; 
    [SerializeReference, SubclassSelector] List<BossPhaseBehavior> mainBehaviors; 
    [SerializeReference, SubclassSelector] List<BossPhaseBehavior> exitBehaviors; 
    


    public float PhaseEndPoint { get => phaseEndPoint; }
    public int PhaseChangeAnim => Animator.StringToHash(phaseChangeAnim.name);

    public List<BossPhaseBehavior> EnterBehaviors { get => enterBehaviors; }
    public List<BossPhaseBehavior> MainBehaviors { get => mainBehaviors; }
    public List<BossPhaseBehavior> ExitBehaviors { get => exitBehaviors; }
    public int PhaseID => phaseID;

}

[CreateAssetMenu(menuName ="Enemy/ Boss")]
public class BossData : EnemySO
{
    [SerializeField] List<BossPhase> phases;
    [SerializeField] AnimationClip weakenedAnimation;

    public int WeakenedAnimation => Animator.StringToHash(weakenedAnimation.name);
    public List<BossPhase> Phases { get => phases; }
}
