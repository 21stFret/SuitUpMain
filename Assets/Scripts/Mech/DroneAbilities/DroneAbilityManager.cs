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
        LoadDroneAbilities();

    }

    private void LoadDroneAbilities()
    {
        for (int i = 0; i < PlayerSavedData.instance.droneLoadOut.Length; i++)
        {
            int index = PlayerSavedData.instance.droneLoadOut[i];
            if (index != -1)
            {
                EquipDroneAbility(index);
            }
        }
    }

    public void UnlockDroneAbility(int index)
    {
        var ability = _droneAbilities[index];
        ability.unlocked = true;
        UpdateDroneAbilityData();
    }

    public void UpdateDroneAbilityData()
    {
        for (int i = 0; i < _droneAbilities.Count; i++)
        {
            PlayerSavedData.instance._droneAbilities[i].unlocked = _droneAbilities[i].unlocked;
            //sAVE EQUIPED  ABILITES
        }
        PlayerSavedData.instance.SavePlayerData();
    }

    public void EquipDroneAbility(int index)
    {
        _equippedAbilities.Add(_droneAbilities[index]);
        UpdateDroneAbilityData();
        droneControllerUI.ActivateDroneInput((DroneType)index);
        //Testing
        /*
        if (_droneAbilities[index].unlocked)
        {
            _equippedAbilities.Add(_droneAbilities[index]);
            UpdateDroneAbilityData();
            droneControllerUI.ActivateDroneInput((DroneType)index);
        }
        else
        {
            Debug.Log("Ability is not unlocked.");
        }
        */
    }
    

    public void UnequipDroneAbility(int index)
    {
        if (_equippedAbilities.Contains(_droneAbilities[index]))
        {
            _equippedAbilities.Remove(_droneAbilities[index]);
            UpdateDroneAbilityData();
        }
        else
        {
            Debug.Log("Ability is not equipped.");
        }
    }
}
