using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningPatch : MonoBehaviour
{
    public DamageArea damageArea;
    public ParticleSystem fireParticles;
    public float burnDuration = 5f;
    private bool _enabled;

    public void EnableDamageArea()
    {
        if (_enabled)
        {
            return;
        }
        _enabled = true;
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
        _enabled = false;
    }
}
