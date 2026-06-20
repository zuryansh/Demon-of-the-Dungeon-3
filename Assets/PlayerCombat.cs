using NUnit.Framework;
using UnityEngine;


//handles which weapon is being used and input handling
public class PlayerCombat : MonoBehaviour, ICombatant
{
    public ICombatHandler CombatHandler => weapon;
    

    [SerializeField] Weapon weapon;
    

    AnimationHelper Animhelper;




    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.TryAttack();
        }    
    }




}
