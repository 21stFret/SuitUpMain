using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DualActionButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Events")]
    public UnityEvent onButtonSelected;    // Triggered when button is selected/highlighted
    public UnityEvent onButtonClicked;     // Triggered when button is clicked/pressed
    public InputTracker inputTracker;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        
        // Set up the click listener
        if (button != null)
        {
            button.onClick.AddListener(() => onButtonClicked.Invoke());
        }
    }

    void Start()
    {
        inputTracker = InputTracker.instance;
    }

    // Called when button is selected via joystick navigation
    public void OnSelect(BaseEventData eventData)
    {
        if((bool)inputTracker?.usingMouse)
        {
            return;
        }
        //onButtonClicked.Invoke();
        button.onClick.Invoke();
    }

    // Called when button loses selection via joystick navigation
    public void OnDeselect(BaseEventData eventData)
    {
        // Optional: Handle deselection if needed
    }

    // Called when mouse enters button
    public void OnPointerEnter(PointerEventData eventData)
    {
        //onButtonSelected.Invoke();
    }

    // Called when mouse exits button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Optional: Handle mouse exit if needed
    }
}
