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

    public override void Die()
    {
        base.Die();
        onFire = true;
        fire.Play();
        fireDamage.enabled = true;
        StartCoroutine(BurnOut());
    }

    public IEnumerator BurnOut()
    {
        yield return new WaitForSeconds(burnTime);
        fire.Stop();
        fireDamage.enabled = false;
        onFire = false;
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
