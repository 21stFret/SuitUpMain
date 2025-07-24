using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidAreaManager : MonoBehaviour
{

    public HealthPickup healthPickup;

    public void InitVoidArea()
    {
        healthPickup.voidPickUp = true;
        healthPickup.Init();
        healthPickup.ResetPickup();
    }
}
