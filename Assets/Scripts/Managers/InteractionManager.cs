using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class InteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference interactUIAction;
    [SerializeField] private InputActionReference interactEndAction;
    private IInteractable currentInteractable;
    public InteractableObject interactableObject;

    private void Awake()
    {
        interactAction.action.Enable();
        interactEndAction.action.Enable();
        interactUIAction.action.Enable();
        interactAction.action.performed += OnInteract;
        interactUIAction.action.performed += OnInteract;
        interactEndAction.action.performed += OnEndInteraction;
    }

    private void OnDestroy()
    {
        interactAction.action.performed -= OnInteract;
        interactUIAction.action.performed -= OnInteract;
        interactEndAction.action.performed -= OnEndInteraction;
        interactUIAction.action.Disable();
        interactAction.action.Disable();
        interactEndAction.action.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactableObject = currentInteractable as InteractableObject;
            currentInteractable.ShowPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            currentInteractable.EndInteraction();
            currentInteractable.ShowPrompt(false);
            currentInteractable = null;
            interactableObject = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.ShowPrompt(false);
            AudioManager.instance.PlaySFX(SFX.Select);
            currentInteractable.Interact();
        }
    }

    private void OnEndInteraction(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.ShowPrompt(true);
            currentInteractable.EndInteraction();
        }
    }
}
