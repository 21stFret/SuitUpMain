using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawVampire : WeaponMod
{
    public ParticleSystem bloodEffect;

    public override void Init()
    {
        base.Init();
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        if (bloodEffect != null)
        {
            bloodEffect.gameObject.SetActive(false);
        }
    }

    public override void Fire()
    {
        base.Fire();
        if (bloodEffect != null)
        {
            bloodEffect.gameObject.SetActive(true);
            bloodEffect.Play();
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (bloodEffect != null)
        {
            bloodEffect.Stop();
            bloodEffect.gameObject.SetActive(false);
        }
    }
}
