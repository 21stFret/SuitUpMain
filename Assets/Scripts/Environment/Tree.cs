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
        if (!_targetHealth.alive)
        {
            return;
        }
        if (burnt)
        {
            return;
        }
        if (killedBy == WeaponType.Chainsaw)
        {
            Chainsawed();
        }
        else
        {
            TriggerOnFire();
        }
        base.Die();
    }

    public void TriggerOnFire()
    {
        if(burnt)
        {
            return;
        }
        StartCoroutine(SetOnFire());
    }

    public void Chainsawed()
    {
        normalRoot.SetActive(false);
        fireRoot.SetActive(false);
        GetComponent<Collider>().enabled = false;
        deadRoot.SetActive(true);
        deadRoot.GetComponent<DeadTree>().Die();
    }

    public IEnumerator SetOnFire()
    {
        burnt = true;
        fireRoot.SetActive(true);
        fireDamage.EnableDamageArea();
        yield return new WaitForSeconds(burnTime);
        normalRoot.transform.DOScale(0, 2);
        GetComponent<Collider>().enabled = false;
        deadRoot.SetActive(true);
        yield return new WaitForSeconds(burnTime - 1);
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
