using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tree : Prop
{
    public GameObject normalRoot;
    public GameObject fireRoot;
    public GameObject deadRoot;
    private bool burnt;
    public float burnTime = 3f;
    public DamageArea fireDamage;

    // Start is called before the first frame update
    void Start()
    {
        burnt = false;
        Init();
    }

    public override void Init()
    {
        base.Init();
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
        fireDamage.EnableDamageArea();
        yield return new WaitForSeconds(burnTime);
        normalRoot.transform.DOScale(0, 2);
        GetComponent<Collider>().enabled = false;
        deadRoot.SetActive(true);
        yield return new WaitForSeconds(burnTime-1);
        fireDamage.SetDamageActive(false);
        fireRoot.SetActive(false);
    }

    public override void RefreshProp()
    {
        base.RefreshProp();
        fireRoot.SetActive(false);
        deadRoot.SetActive(false);
        normalRoot.transform.localScale = Vector3.one;
        burnt = false;
        normalRoot.SetActive(true);
    }
}
