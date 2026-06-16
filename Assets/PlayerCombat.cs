using NUnit.Framework;
using UnityEngine;


//handles which weapon is being used and input handling
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    

    AnimationHelper Animhelper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.TryAttack();
        }    
    }




}
