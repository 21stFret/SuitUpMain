using UnityEngine;
using TMPro;

public class HintManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public DoTweenFade fadeInOut;
    
    private readonly string[] hints = new string[]
    {
        "Tip: Use dash to quickly dodge enemy attacks!",
        "Tip: Remember you can Pulse to push Crawlers away!",
        "Tip: Different weapons have different attack patterns.",
        "Tip: Watch your health and energy levels!",
        "Tip: Collect artifacts to upgrade your weapons and mech.",
        "Tip: Use the environment to your advantage in battles.",
        "Tip: If lost, remain motionless to show the objective tracker.",
        "Tip: Watch out! Some Crawlers explode on death!",
        "Tip: Keep moving to stay alive!",
        "Tip: Visit the Workshop to upgrade your weapons.",
        "Tip: Energy recharges over time.",
        "Tip: Look for pickups in crates during battles!",
        "Tip: Drone attacks can shift the flow of battle!",
    };

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ShowRandomHint()
    {
        fadeInOut.FadeIn();
        if (hintText != null)
        {
            int randomIndex = Random.Range(0, hints.Length);
            hintText.text = hints[randomIndex];
        }
    }

    public void HideHint()
    {
        fadeInOut.FadeOut();
    }
}