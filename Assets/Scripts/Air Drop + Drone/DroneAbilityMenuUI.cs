using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroneAbilityMenuUI : MonoBehaviour
{
    public int droneAbilityID;
    public bool unlocked;
    public GameObject lockedIcon;
    public TMP_Text abilityName;
    public TMP_Text abilityDescription;
    public TMP_Text abilityChargeDescription;
    public TMP_Text[] chargeInts;
    public TMP_Text cost;
}
