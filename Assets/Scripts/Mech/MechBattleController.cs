using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBattleController : MonoBehaviour
{
    public static MechBattleController instance;
    public TargetHealth mechHealth;
    public MYCharacterController characterController;

    private void Awake()
    {
        instance = this;
    }

    public void OnDie()
    {
        characterController.Die();
        characterController.enabled = false;
        GameManager.instance.GameOver();
        gameObject.layer = 1;
    }

    public void RepairArmour()
    {
        mechHealth.TakeDamage(-50, null);
        print("Repaired Amrour");
    }

}
