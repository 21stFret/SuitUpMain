using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;

    public void SwitchToVirtualCamera(int index)
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i == index)
            {
                virtualCameras[i].Priority = 10;
            }
            else
            {
                virtualCameras[i].Priority = 0;
            }
        }
    }
}
