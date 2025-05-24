using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAbilityManager : MonoBehaviour
{
    public static DroneAbilityManager instance;
    public DroneControllerUI droneControllerUI;
    public List<DroneAbility> _droneAbilities = new List<DroneAbility>();
    public DroneAbilityDataReader dataReader;
    public List<DroneAbility> _equippedAbilities = new List<DroneAbility>();
    public bool equipAbilitesOnAwake = false;

    [InspectorButton("TEquipDroneAbility")]
    public bool equipDroneAbility;
    public int equipIndex;
    public void TEquipDroneAbility()
    {
        EquipDroneAbility(equipIndex);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        dataReader.LoadFromExcel(this);
        // Load the drone loadout from PlayerSavedData
        UpdateDroneAbilityData();
        LoadDroneAbilities();
    }

    private void LoadDroneAbilities()
    {
        for (int i = 0; i < PlayerSavedData.instance.droneLoadOut.Length; i++)
        {
            int index = PlayerSavedData.instance.droneLoadOut[i];
            if (index >= 0)
            {
                if(equipAbilitesOnAwake)
                {
                    EquipDroneAbility(index);
                }
            }
        }
    }

    public void UpdateDroneAbilityData()
    {
        for (int i = 0; i < _droneAbilities.Count; i++)
        {
            _droneAbilities[i].unlocked = PlayerSavedData.instance._droneAbilities[i] == 0 ? true : false;
        }
    }

    public void EquipDroneAbility(int index)
    {
        _equippedAbilities.Add(_droneAbilities[index]);
        droneControllerUI.ActivateDroneInput((DroneType)index);
    }
    

}
