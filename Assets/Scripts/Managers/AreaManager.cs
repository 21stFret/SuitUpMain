using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaType
{
    Grass,
    Desert,
    Ice,
    Jungle,
}

public class AreaManager : MonoBehaviour
{
    public List<GameObject> GrassRoomPrefabs = new List<GameObject>();
    public List<GameObject> DesertRoomPrefabs = new List<GameObject>();
    public List<GameObject> IceRoomPrefabs = new List<GameObject>();
    public List<GameObject> JungleRoomPrefabs = new List<GameObject>();

    public GameObject voidArea;

    public GameObject currentRoom;

    public void LoadRoom(AreaType areaType)
    {
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
        }

        if (roomPrefabs == null)
        {
            Debug.LogError("No room prefabs found for area type: " + areaType);
            return;
        }

        int randomIndex = Random.Range(0, roomPrefabs.Count);
        GameObject roomPrefab = roomPrefabs[randomIndex];
        if (roomPrefab == currentRoom)
        {
            print("Room is the same as the current room");
            randomIndex = (randomIndex + 1) % roomPrefabs.Count;
        }
        TreeClusterGeneration levelGen = roomPrefab.GetComponentInChildren<TreeClusterGeneration>();
        if (levelGen != null) levelGen.GenerateTreeClusters();
        roomPrefab.SetActive(true);
        currentRoom = roomPrefab;
    }

    public void LoadVoidArea()
    {
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }
        voidArea.SetActive(true);
        currentRoom = voidArea;
    }

}
