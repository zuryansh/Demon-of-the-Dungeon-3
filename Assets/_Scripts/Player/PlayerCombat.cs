using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using EditorAttributes;

//handles which weapon is being used and input handling
public class PlayerCombat : MonoBehaviour, ICombatant
{
    public ICombatHandler CombatHandler => currentWeapon;
    

    [HideProperty] public UnityEvent<WeaponData> EOnWeaponChanged;
    [SerializeField] List<Weapon> weapons;
    [SerializeField] Weapon currentWeapon;
    [SerializeReference, SubclassSelector] List<Effect> onHitEffects;

    AnimationHelper Animhelper;
    Player player;



    private void Start()
    {
        player = Player.Instance;
        currentWeapon = weapons[0];
    }

    float lastAutoAttackTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastAutoAttackTime >= currentWeapon.Data.TimeBetweenAutoAttack && player.IsUsingJoystick )
        {
            lastAutoAttackTime = Time.time;
            currentWeapon.TryAttack();
        }
        

        //if(Input.GetMouseButtonDown(0))
        //{
        //    currentWeapon.TryAttack();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        //else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        //else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        //else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeapon(3);

    }

    int weaponIndex = 0;
    public void CycleWeapon()
    {
        weaponIndex++;
        weaponIndex %= weapons.Count;
        ChangeWeapon(weaponIndex);
    }

    void ChangeWeapon(int i)
    {
        weaponIndex = i;
        currentWeapon = weapons[i];
        EOnWeaponChanged?.Invoke(currentWeapon.Data);
    }

    public void HandleAttackInput(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            currentWeapon.TryAttack();
        }
    }

    public void HandleWeaponChangeInput(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            CycleWeapon();
        }
    }



    public void OnHit(EffectContext context)
    {
        foreach (Effect effect in onHitEffects)
        {
            effect.Apply(context);
        }
    }

}
