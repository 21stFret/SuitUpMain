using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPatch : MonoBehaviour
{
    public ParticleSystem particleEffect;
    private bool triggerd = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerd)
        {
            if (particleEffect != null)
            {
                particleEffect.Play();
                triggerd = true;
                transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutBack);
            }
            else
            {
                Debug.LogWarning("Particle effect is not assigned.");
            }
        }
    }

}
