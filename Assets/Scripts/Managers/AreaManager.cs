using System.Collections;
using System.Collections.Generic;
using FORGE3D;
using Unity.VisualScripting;
using UnityEngine;

public enum AreaType
{
    Grass,
    Desert,
    Ice,
    Jungle,
    Void,
}

public class AreaManager : MonoBehaviour
{
    public List<GameObject> GrassRoomPrefabs = new List<GameObject>();
    public List<GameObject> DesertRoomPrefabs = new List<GameObject>();
    public List<GameObject> IceRoomPrefabs = new List<GameObject>();
    public List<GameObject> JungleRoomPrefabs = new List<GameObject>();
    private List<GameObject> allRooms = new List<GameObject>();

    public GameObject voidArea;
    public GameObject currentRoom;

    public GameObject dayLight;
    public GameObject nightLight;
    public DirectionalDaylight directionalDaylight;
    public DataLogSpawner dataLogSpawner;
    public F3DMissileLauncher missileLauncher;

    private void Start()
    {
        allRooms.AddRange(GrassRoomPrefabs);
        allRooms.AddRange(DesertRoomPrefabs);
        allRooms.AddRange(IceRoomPrefabs);
        allRooms.AddRange(JungleRoomPrefabs);
    }

    public void DayNightCycle(bool night = false)
    {
        dayLight.SetActive(!night);
        nightLight.SetActive(night);
        directionalDaylight.enabled = !night;
        directionalDaylight.ApplyLightSettings();
    }

    public void LoadRoom(AreaType areaType)
    {
        CashCollector.instance.DestroyParts();
        missileLauncher.RefreshMines();
        allRooms.ForEach(room => room.SetActive(false));
        EnvironmentArea area = null;
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
            area = currentRoom.GetComponent<EnvironmentArea>();
            if (area != null)
            {
                area.RefreshArea();
            }

        }
        List<GameObject> roomPrefabs = null;
        switch (areaType)
        {
            case AreaType.Grass:
                roomPrefabs = GrassRoomPrefabs;
                break;
            case AreaType.Desert:
                roomPrefabs = DesertRoomPrefabs;
                break;
            case AreaType.Ice:
                roomPrefabs = IceRoomPrefabs;
                break;
            case AreaType.Jungle:
                roomPrefabs = JungleRoomPrefabs;
                break;
            case AreaType.Void:
                LoadVoidArea();
                break;
        }

        if (roomPrefabs == null)
        {
            Debug.LogError("No room prefabs found for area type: " + areaType);
            return;
        }

        GameObject roomPrefab;

        if (BattleManager.instance._usingBattleType == BattleType.Hunt)
        {
            roomPrefab = roomPrefabs[roomPrefabs.Count - 1];
        }
        else
        {
            // leave the final room as the boss room
            int randomIndex = Random.Range(0, roomPrefabs.Count - 1);
            roomPrefab = roomPrefabs[randomIndex];
            if (roomPrefab == currentRoom)
            {
                print("Room is the same as the current room");
                randomIndex = (randomIndex + 1) % (roomPrefabs.Count - 1);
                roomPrefab = roomPrefabs[randomIndex];
            }

            area = roomPrefab.GetComponent<EnvironmentArea>();
            
            if (directionalDaylight != null)
            {
                bool dark = false;
                if (area != null)
                {
                    dark = area.insideArea;
                }
                // If the area is a battle type and it's a survival battle, set dark to true 
                if (BattleManager.instance._usingBattleType == BattleType.Survive)
                {
                    dark = true;
                }
                DayNightCycle(dark);
            }
            TreeClusterGeneration levelGen = roomPrefab.GetComponentInChildren<TreeClusterGeneration>();
            if (levelGen != null) levelGen.GenerateTreeClusters();
        }

        roomPrefab.SetActive(true);
        currentRoom = roomPrefab;
        LoadDataLog();
    }

    public void LoadVoidArea()
    {
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
            EnvironmentArea area = currentRoom.GetComponent<EnvironmentArea>();
            if (area != null)
            {
                area.RefreshArea();
            }
        }
        voidArea.SetActive(true);
        currentRoom = voidArea;
        DayNightCycle(true);
    }
    
    public void LoadDataLog()
    {
        int randomIndex = Random.Range(0, 100);
        if (randomIndex > 10)
        {
            dataLogSpawner.HideDataLog();
            return;
        }
        if (dataLogSpawner != null && currentRoom != null)
        {
            dataLogSpawner.SpawnRandomDataLog();
        }
        else
        {
            Debug.LogError("DataLogSpawner or currentRoom is null. Cannot load data log.");
        }
    }
}
