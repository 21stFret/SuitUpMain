using System.Collections;
using System.Collections.Generic;
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
        directionalDaylight.startTime = Random.Range(0.2f, 0.7f);
        directionalDaylight.Init();
    }

    public void LoadRoom(AreaType areaType)
    {
        allRooms.ForEach(room => room.SetActive(false));

        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
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
        EnvironmentArea area;

        if(BattleManager.instance._usingBattleType == BattleType.Hunt)
        {
            roomPrefab = roomPrefabs[roomPrefabs.Count - 1];
            area = roomPrefab.GetComponent<EnvironmentArea>();
        }
        else
        {
            // leave the final room as the boss room
            int randomIndex = Random.Range(0, roomPrefabs.Count-1);
            roomPrefab = roomPrefabs[randomIndex];
            if (roomPrefab == currentRoom)
            {
                print("Room is the same as the current room");
                randomIndex = (randomIndex + 1) % (roomPrefabs.Count - 1);
                roomPrefab = roomPrefabs[randomIndex];
            }
            area = roomPrefab.GetComponent<EnvironmentArea>();
            if(directionalDaylight!=null)
            {
                bool dark = area.insideArea;
                if(BattleManager.instance._usingBattleType == BattleType.Survive)
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
        area.RefreshArea();
        LoadDataLog();
    }

    public void LoadVoidArea()
    {
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }
        voidArea.SetActive(true);
        currentRoom = voidArea;
        DayNightCycle(true);
    }
    
    public void LoadDataLog()
    {
        int randomIndex = Random.Range(0, 100);
        if (randomIndex < 25)
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
