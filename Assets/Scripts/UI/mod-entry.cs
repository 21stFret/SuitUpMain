using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ModEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image modIcon;
    [SerializeField] private Image modRaityGlow;
    [SerializeField] private GameObject conector; // Reference to the popup GameObject
    public Vector3 popupOffset; // Offset for the popup position
    public PauseModUI pauseModUI;
    public RunMod _mod;
    public bool flipInfoWIndow = false;
    private bool _init;

    private void Start()
    {
        if (_init)
        {
            return;
        }
        _init = true;
        if (flipInfoWIndow)
        {
            popupOffset.x = -540;
            Vector3 scale = conector.transform.localScale;
            scale.x = -scale.x;
            conector.transform.localScale = scale;
            Vector2 pos = conector.transform.localPosition;
            pos.x -= 35;
            conector.transform.localPosition = pos;
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


    public void OnHoverEnter()
    {
        pauseModUI.SetupText(_mod);
        conector.SetActive(true); // Show connector on hove
        Vector3 vector3 = transform.localPosition;
        pauseModUI.SetPopupPosition(vector3 + popupOffset);
    }

    public void OnHoverExit()
    {
        conector.SetActive(false); // Hide connector when not hovering
        pauseModUI.infoPopup.SetActive(false); // Hide popup when not hovering
    }


}
