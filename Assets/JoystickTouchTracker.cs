using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouchTracker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed => isPressed;

    [SerializeField] bool isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}