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


    private void Start()
    {
        currentWeapon = weapons[0];
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    currentWeapon.TryAttack();
        //}
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeapon(3);

    }
    
    
    void ChangeWeapon(int i)
    {
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

    


    public void OnHit(EffectContext context)
    {
        foreach (Effect effect in onHitEffects)
        {
            effect.Apply(context);
        }
    }

}
