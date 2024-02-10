using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public DoTweenScale doTweenScale;
    public ParticleSystem _particleSystem;
    public ParticleSystem _particleSystem2;
    public FORGE3D.F3DWarpJumpTunnel[] f3DWarpJumpTunnel;

    public void StartEffect()
    {
        for(int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].OnSpawned();
        }
        doTweenScale.StartTween();
        _particleSystem.Play();
        _particleSystem2.Play();
    }
}
