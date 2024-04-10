using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public DoTweenScale doTweenScale;
    public ParticleSystem _particleSystem;
    public ParticleSystem _particleSystem2;
    public FORGE3D.F3DWarpJumpTunnel[] f3DWarpJumpTunnel;
    public float portalEffectDuration = 1f;
    public bool infinite;

    public void StartEffect()
    {
        for(int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].FadeDelay = portalEffectDuration;
            f3DWarpJumpTunnel[i].infinite = infinite;
            f3DWarpJumpTunnel[i].OnSpawned();
        }
        doTweenScale.StartTween();
        _particleSystem.Play();
        _particleSystem2.Play();
        if(!infinite)
        {
            StartCoroutine(DealyedStopEffect());
        }
    }

    public void StopEffect()
    {
        for(int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].ToggleGrow();
        }
        doTweenScale.ReverseTween();
        _particleSystem.Stop();
        _particleSystem2.Stop();
    }


    private IEnumerator DealyedStopEffect()
    {
        yield return new WaitForSeconds(portalEffectDuration);
        doTweenScale.ReverseTween();
        _particleSystem.Stop();
        _particleSystem2.Stop();
    }

}
