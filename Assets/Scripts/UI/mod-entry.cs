using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image modIcon;
    [SerializeField] private TextMeshProUGUI modNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI rarityText;

    public void SetupMod(RunMod mod)
    {
        if (modIcon != null && mod.sprite != null)
            modIcon.sprite = mod.sprite;

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
    }
}
