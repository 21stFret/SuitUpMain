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
        if(GameManager.instance != null)
        {
            GameManager.instance.EndGameCall(false);
        }

        gameObject.layer = 1;
    }



    public void RepairArmour()
    {
        mechHealth.TakeDamage(-50, WeaponType.Minigun);
        print("Repaired Amrour");
    }

}
