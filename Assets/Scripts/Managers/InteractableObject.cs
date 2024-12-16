using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Component for interactable objects
public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onEnd;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private Image interactPrompt;
    [SerializeField] private Sprite controlPC, controlGamepad;

    public void Interact()
    {
        if (CanInteract())
        {
            onInteract?.Invoke();
        }
    }

    public void EndInteraction()
    {
        onEnd?.Invoke();
    }

    public bool CanInteract()
    {
        return isInteractable;
    }

    public void ShowPrompt(bool on)
    {
        interactPrompt.enabled = on;
        if (InputTracker.instance != null)
        {
            interactPrompt.sprite = InputTracker.instance.usingMouse ? controlPC : controlGamepad;
        }
    }
}


// Interface for interactable objects
public interface IInteractable
{

    void Interact();
    bool CanInteract();
    void ShowPrompt(bool on);
    void EndInteraction();
}
