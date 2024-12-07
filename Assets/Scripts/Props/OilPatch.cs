using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilPatch : Prop
{
    private MYCharacterController player;
    private float savedSpeed;
    public bool onFire;
    public ParticleSystem fire;
    public DamageArea fireDamage;
    public float burnTime = 5f;
    public GameObject oilPatch;

    private float refScale;

    public override void Die()
    {
        base.Die();
        onFire = true;
        fire.Play();
        fireDamage.EnableDamageArea();
        StartCoroutine(BurnOut());
    }

    public IEnumerator BurnOut()
    {
        refScale = oilPatch.transform.localScale.x;
        yield return new WaitForSeconds(1f);
        oilPatch.transform.DOScale(0.2f, burnTime-1);
        yield return new WaitForSeconds(burnTime-1);
        fire.Stop();
        fireDamage.damageActive = false;
        onFire = false;
        StartCoroutine(RefreshOil());
    }

    private IEnumerator RefreshOil()
    {
        yield return new WaitForSeconds(burnTime);
        oilPatch.transform.DOScale(refScale, burnTime);
        yield return new WaitForSeconds(burnTime);
        RefreshProp();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {
                return;
            }
            player = other.gameObject.GetComponent<MYCharacterController>();
            savedSpeed = player.Speed;
            player.Speed = player.Speed / 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player == null)
            {
                return;
            }
            player.Speed = savedSpeed;
            player = null;
        }
    }
}
