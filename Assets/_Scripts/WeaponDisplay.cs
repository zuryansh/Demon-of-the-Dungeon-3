using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    [SerializeField] Image displayImage;
    UnityEvent<WeaponData> registeredEvent;


    public void RegisterEvent(UnityEvent<WeaponData> ev)
    {
        registeredEvent = ev;
        registeredEvent.AddListener(UpdateImage);
    }
    

    void UpdateImage(WeaponData data)
    {

        displayImage.sprite = data.Icon;
        
    }

    private void OnDisable()
    {
        if (registeredEvent != null) { registeredEvent.RemoveListener(UpdateImage); }
    }
}
