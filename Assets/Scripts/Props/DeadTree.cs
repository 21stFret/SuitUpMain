using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeadTree : Prop
{
    BreakableObject breakableObject;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Die()
    {
        breakableObject.transform.parent = null;
        breakableObject.Break();
        gameObject.SetActive(false);
    }
}
