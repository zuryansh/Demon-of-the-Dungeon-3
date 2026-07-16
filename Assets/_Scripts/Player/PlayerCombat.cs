using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//handles which weapon is being used and input handling
public class PlayerCombat : MonoBehaviour, ICombatant
{
    public ICombatHandler CombatHandler => currentWeapon;
    

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
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = weapons[0];
        else if(Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = weapons[1];
        else if(Input.GetKeyDown(KeyCode.Alpha3)) currentWeapon = weapons[2];
    }

    public void HandleAttackInput(InputAction.CallbackContext c)
    {
        if (c.started)
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
