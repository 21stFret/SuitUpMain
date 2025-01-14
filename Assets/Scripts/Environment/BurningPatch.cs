using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningPatch : MonoBehaviour
{
    public DamageArea damageArea;
    public ParticleSystem fireParticles;
    public float burnDuration = 5f;

    public void EnableDamageArea()
    {
        damageArea.damageDuration = burnDuration;
        damageArea.EnableDamageArea();
        fireParticles.Play();
        StartCoroutine(DisableDamageArea());
    }

    public IEnumerator DisableDamageArea()
    {
        yield return new WaitForSeconds(burnDuration);
        fireParticles.Stop();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
