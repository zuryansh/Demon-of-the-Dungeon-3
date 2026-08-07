using UnityEngine;

public class SlimeBoss : Boss
{
    protected override void RegisterBossAttacks()
    {
        base.RegisterBossAttacks();
        RegisterAttack<ChargeAttackData>(Charge);
        RegisterAttack<JumpAttackData>(Jump);
        RegisterAttack<ProjectileAttackData>(BurstProjectile);
    }
}
