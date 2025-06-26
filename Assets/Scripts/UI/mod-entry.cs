using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ModEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image modIcon;
    [SerializeField] private Image modRaityGlow;
    public Vector3 popupOffset; // Offset for the popup position
    public PauseModUI pauseModUI;
    public RunMod _mod;
    public bool flipInfoWIndow = false;
    public bool _init;
    public ChipSlotUI chipSlotUI;
    public CustomHoverButton customHoverButton;

    private void Start()
    {
        if (_init)
        {
            return;
        }
        _init = true;
        customHoverButton = GetComponent<CustomHoverButton>();
        if (customHoverButton != null)
        {
            customHoverButton.onClick.AddListener(() =>
            {
                ClickThrough();
            });
        }
    }

    public void SetupMod(RunMod mod)
    {
        if (modIcon != null && mod.sprite != null)
        {
            modIcon.sprite = mod.sprite;
            modIcon.color = Color.white;
        }

        if (modRaityGlow != null)
        {
            Color rarityColor = mod.rarity switch
            {
                0 => Color.white,
                1 => Color.cyan,
                2 => Color.magenta,
                _ => Color.white
            };
            modRaityGlow.color = rarityColor;
            modRaityGlow.enabled = true;
        }
        _mod = mod;

    }

    public void ClickThrough()
    {
        if (chipSlotUI != null)
        {
            chipSlotUI.InteractWithSlot();
        }
    }


    public void OnHoverEnter()
    {
        pauseModUI.SetupText(_mod);
        Vector3 vector3 = transform.localPosition;
        pauseModUI.SetPopupPosition(vector3 + popupOffset);
    }

    public void OnHoverExit()
    {
        pauseModUI.infoPopup.SetActive(false); // Hide popup when not hovering
    }


}
