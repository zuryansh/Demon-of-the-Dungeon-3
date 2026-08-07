using UnityEngine;

[CreateAssetMenu(menuName = "Attack/ Burst Proj Attack")]
public class ProjectileAttackData : AttackData
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int noOfProf;
    [SerializeField, Range(0, 360)] float coverAngle;
    [SerializeField] float projSpeed;
    [SerializeField] float timeBetweenProj;
    [SerializeField] float randomAngleOffset;

    public Projectile ProjectilePrefab { get => projectilePrefab; }
    public int NoOfProj { get => noOfProf; }
    public float CoverAngle { get => coverAngle; }
    public float ProjSpeed { get => projSpeed; }
    public float TimeBetweenProj => timeBetweenProj;
    public float RandomAngleOffset { get => randomAngleOffset; }

}
