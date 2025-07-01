using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockUIPopup : MonoBehaviour
{
    public List<LockColorsUI> lockColors = new List<LockColorsUI>();

    public void SetupLockInfo(List<ComboLock> combolocks, List<RunMod> runMods)
    {
        for(int i = 0; i < lockColors.Count; i++)
        {
            if (i < combolocks.Count)
            {
                lockColors[i].SetColor(combolocks[i]);
                lockColors[i].SetText(runMods[i].modName);
            }
            else
            {
                lockColors[i].gameObject.SetActive(false); // Hide unused lock colors
            }
        }
    }
}
