using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerData : MonoBehaviour
{
    public int cash;
    public int artifact;
    public Vector2 playerLoadout;
    public WeaponData[] mainWeaponData;
    public WeaponData[] altWeaponData;

    private void Start()
    {
        PlayerSavedData.instance.CreateData();
        PlayerSavedData.instance.UpdatePlayerCash(cash);
        PlayerSavedData.instance.UpdatePlayerArtifact(artifact);
        PlayerSavedData.instance.UpdateMainWeaponLoadout((int)playerLoadout.x);
        PlayerSavedData.instance.UpdateAltWeaponLoadout((int)playerLoadout.y);
        PlayerSavedData.instance.UpdateMainWeaponData(mainWeaponData[0], (int)playerLoadout.x);
        PlayerSavedData.instance.UpdateAltWeaponData(altWeaponData[0], (int)playerLoadout.y);
    }
}
