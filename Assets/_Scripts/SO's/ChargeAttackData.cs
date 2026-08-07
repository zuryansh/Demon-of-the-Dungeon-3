using UnityEngine;

[CreateAssetMenu(menuName = "Attack/ ChargeAttack")]
public class ChargeAttackData : AttackData
{
    [SerializeField] float chargeSpeed;
    [SerializeField] float maxChargeTime;

    public float ChargeSpeed { get => chargeSpeed; }
    public float MaxChargeTime { get => maxChargeTime; }

}

