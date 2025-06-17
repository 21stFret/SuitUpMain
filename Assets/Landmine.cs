using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : ExplodingBarrel
{
    private bool triggered;
    public void Awake()
    {
        var colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.isTrigger)
            {
                continue;
            }
            _collider = col;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (other.gameObject.tag == "Enemy")
        {
            Die();
            triggered = true;
        }
    }

    public override void Init()
    {
        base.Init();
        gameObject.SetActive(true);
        StartCoroutine(DelayTrigger());
    }

    private IEnumerator DelayTrigger()
    {
        yield return new WaitForSeconds(1f);
        triggered = false;
    }

    public override void RefreshProp()
    {

    }
}
