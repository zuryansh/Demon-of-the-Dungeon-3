using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] SliderController playerHealthBar;
    [SerializeField] PlayerCounterText pointsCounter;
    [SerializeField] WeaponDisplay weaponDisplay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Player.Instance;
        if (player == null) Debug.LogError("PLAYER NOT FOUND FOR HUD");

        if (playerHealthBar != null) playerHealthBar.RegisterHealthBarUser(player.HealthScript);
        if (pointsCounter != null) pointsCounter.RegisterCounterTo(player.EOnPointsChanged);
        if (weaponDisplay != null) weaponDisplay.RegisterEvent(player.CombatScript.EOnWeaponChanged);
    
    }



}
