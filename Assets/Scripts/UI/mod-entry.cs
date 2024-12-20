using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ModEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image modIcon;
    [SerializeField] private Image modRaityGlow;
    [SerializeField] private TextMeshProUGUI modNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private GameObject popup; // Reference to the popup GameObject

    private void Start()
    {
        if (popup != null)
            popup.SetActive(false); // Ensure popup is hidden at start
    }

    public void SetupMod(RunMod mod)
    {
        if (modIcon != null && mod.sprite != null)
        {
            modIcon.sprite = mod.sprite;
            modIcon.color = Color.white;
        }


        if (modNameText != null)
            modNameText.text = mod.modName;

        if (descriptionText != null)
        {
            string description = mod.modDescription;
            foreach (Modifier modifier in mod.modifiers)
            {
                description += $"\n• {modifier.statType}: {modifier.statValue:F2}";
            }
            descriptionText.text = description;
        }

        if (categoryText != null)
            categoryText.text = $"Category: {mod.modCategory}";

        if (rarityText != null)
        {
            string rarityString = mod.rarity switch
            {
                0 => "Common",
                1 => "Rare",
                2 => "Epic",
                _ => "Unknown"
            };
            rarityText.text = $"Rarity: {rarityString}";
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

    }

    private void OnPointerExit(PointerEventData data)
    {
        OnHoverExit();
    }

    public void OnHoverEnter()
    {
        if (popup != null)
            popup.SetActive(true); // Show popup on hover
    }

    public void OnHoverExit()
    {
        if (popup != null)
            popup.SetActive(false); // Hide popup when not hovering
    }
}
