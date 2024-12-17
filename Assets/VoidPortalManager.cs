using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoidPortalManager : MonoBehaviour
{
    public PortalEffect[] portalEffects;
    public PortalEffect voidPortalEffect;
    public int MaxPortalEffects = 3;
    public RunUpgradeManager runUpgradeManager;
    public Transform voidPortalLocation;

    [InspectorButton("StartEffect")]
    public bool startEffect;

    public void StartEffect()
    {
        runUpgradeManager.SelectNextBuilds();
        for (int i = 0; i < portalEffects.Length; i++)
        {
            portalEffects[i].gameObject.SetActive(true);
            SetPortalColor(portalEffects[i], runUpgradeManager.randomlySelectedBuilds[i]);
            portalEffects[i].StartEffect();
            RoomPortal portal = portalEffects[i].GetComponent<RoomPortal>();
            portal.portalType = runUpgradeManager.randomlySelectedBuilds[i];
            portal._active = true;
        }
    }

    public void StartVoidEffect()
    {
        voidPortalEffect.StartEffect();
        RoomPortal portal = voidPortalEffect.GetComponent<RoomPortal>();
        portal.voidPortal = true;
        portal._active = true;
    }

    public void StopEffect()
    {
        for (int i = 0; i < portalEffects.Length; i++)
        {
            if (portalEffects[i].isActive)
            {
                portalEffects[i].StopEffect();
                portalEffects[i].GetComponent<RoomPortal>()._active = false;
            }
        }
    }

    public static void SetPortalColor(PortalEffect portal, ModBuildType type)    
    {
        Color color = Color.red;
        switch(type)
        {
            case ModBuildType.ASSAULT:
                color = Color.red;
                break;
            case ModBuildType.TECH:
                color = Color.cyan;
                break;
            case ModBuildType.TANK:
                color = Color.green;
                break;
            case ModBuildType.AGILITY:
                color = Color.yellow;
                break;
        }
        for (int i = 0; i < portal.f3DWarpJumpTunnel.Length; i++)
        {
            Material mat = portal.f3DWarpJumpTunnel[i].GetComponent<Renderer>().material;
            mat.SetColor("_TintColorA", color);
            Color lighter = LightenColor(color, 30);
            mat.SetColor("_TintColorB", lighter);

        }
        var particleSytems = portal.GetComponentsInChildren<ParticleSystemRenderer>();
        foreach (var ps in particleSytems)
        {
            ps.material.SetColor("_TintColorA", color);
            Color lighter = LightenColor(color, 30);
            ps.material.SetColor("_TintColorB", lighter);
        }
    }

    public static Color LightenColor(Color color, float percent)
    {
        // Ensure the percentage is between 0 and 100
        percent = Mathf.Clamp(percent, 0, 100);

        // Calculate the factor to lighten by
        float factor = percent / 100;

        // Calculate the new RGB values
        float r = color.r + (1.0f - color.r) * factor;
        float g = color.g + (1.0f - color.g) * factor;
        float b = color.b + (1.0f - color.b) * factor;

        // Return the lightened color
        return new Color(r, g, b, color.a);
    }
}
