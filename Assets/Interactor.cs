using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] Interactable selectedInteractor;
    [SerializeField] float radius;
    [SerializeField] GameObject interactText;


    public void OnInteractableClose(Interactable interactable)
    {
        selectedInteractor = interactable;
        interactText.SetActive(true);
    }

    public void OnInteractableFar()
    {
        selectedInteractor = null;
        interactText.SetActive(false);

    }

    private void Update()
    {
        if (selectedInteractor != null)
        {
            if ((selectedInteractor.transform.position - transform.position).sqrMagnitude > radius * radius)
            {
                OnInteractableFar();
            }
        }



    }

    public void HandleInterctInput(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            if (selectedInteractor != null)
            {
                InitiateInteraction(selectedInteractor);
            }
        }
    }

    private void InitiateInteraction(Interactable interactable)
    {
        interactable.Interact(gameObject);
    }
}
