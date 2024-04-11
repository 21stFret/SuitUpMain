using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Prop
{
    public GameObject fireRoot;
    public GameObject deadRoot;
    private bool burnt;
    public float burnTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        burnt = false;
    }

    public override void Die()
    {
        if(burnt)
        {
            return;
        }
        TriggerOnFire();
    }

    public void TriggerOnFire()
    {
        if(burnt)
        {
            return;
        }
        StartCoroutine(SetOnFire());
    }

    public IEnumerator SetOnFire()
    {
        burnt=true;
        fireRoot.SetActive(true);
        yield return new WaitForSeconds(burnTime);
        GetComponent<MeshRenderer>().enabled = false;
        deadRoot.SetActive(true);
        yield return new WaitForSeconds(burnTime-1);
        fireRoot.SetActive(false);
    }
}
