using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerData : MonoBehaviour
{
    public int cash;
    public int artifact;
    public Vector2 playerLoadout;
    public int[] droneLoadout;
    public WeaponData[] mainWeaponData;
    public WeaponData[] altWeaponData;
    public float BGMVolume;
    public float SFXVolume;

    public void InittestData()
    {
        PlayerSavedData playerSavedData = PlayerSavedData.instance;
        playerSavedData.CreateData();
        playerSavedData.UpdatePlayerCash(cash);
        playerSavedData.UpdateDroneLoadout(droneLoadout);
        playerSavedData.UpdatePlayerArtifact(artifact);
        playerSavedData.UpdateMainWeaponLoadout((int)playerLoadout.x);
        playerSavedData.UpdateAltWeaponLoadout((int)playerLoadout.y);
        playerSavedData.UpdateMainWeaponData(mainWeaponData[0], (int)playerLoadout.x);
        playerSavedData.UpdateAltWeaponData(altWeaponData[0], (int)playerLoadout.y);
        playerSavedData.UpdateBGMVolume(BGMVolume);
        playerSavedData.UpdateSFXVolume(SFXVolume);
    }
}
