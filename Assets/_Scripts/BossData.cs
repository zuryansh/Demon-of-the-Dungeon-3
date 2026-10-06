using EditorAttributes;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class BossPhase
{
    [SerializeField] List<AttackData> attacks;
    [SerializeField, Range(0, 1)] float phaseEndPoint; 
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int maxNoOfAttacks;
    [SerializeField] int minNoOfAttacks;
    [SerializeField] float timeBetweenRotations;
    [SerializeField] AnimationClip phaseChangeAnim;
    


    public List<AttackData> Attacks { get => attacks; }
    public float PhaseEndPoint { get => phaseEndPoint; }
    public float TimeBetweenAttacks { get => timeBetweenAttacks; }
    public int MaxNoOfAttacks { get => maxNoOfAttacks; }
    public int MinNoOfAttacks { get => minNoOfAttacks; }
    public float TimeBetweenRotations { get => timeBetweenRotations; }
    public int PhaseChangeAnim => Animator.StringToHash(phaseChangeAnim.name);

    public List<AttackData> GetRandomAttacks()
    {
        int n = UnityEngine.Random.Range(minNoOfAttacks, maxNoOfAttacks+1);
        List<AttackData> datas = new();
        for (int i = 0; i < n; i++)
        {
            datas.Add(attacks.Choice());
        }
        return datas;
    }
}

[CreateAssetMenu(menuName ="Enemy/ Boss")]
public class BossData : EnemySO
{
    [SerializeField] List<BossPhase> phases;
    [SerializeField] AnimationClip weakenedAnimation;

    public int WeakenedAnimation => Animator.StringToHash(weakenedAnimation.name);
    public List<BossPhase> Phases { get => phases; }
}
