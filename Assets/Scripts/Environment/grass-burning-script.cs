using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GrassBurning : Prop
{
    public float spreadRadius = 2f;
    public float ignitionDelay = 1f;
    public float burnDuration = 5f;
    public GameObject GrassPrefab;
    public GameObject burnedGrassPrefab;
    public ParticleSystem fireEffect;
    public DamageArea fireDamage;
    public Light fireLight;

    private bool isIgnited = false;
    private bool isOnFire = false;
    private bool isBurned = false;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        GrassPrefab.SetActive(true);
        burnedGrassPrefab.SetActive(false);
        fireEffect.gameObject.SetActive(false);
        isBurned = false;
        isIgnited = false;
        isOnFire = false;
    }

    public override void Die()
    {
        if (isBurned) return;
        base.Die();
        fireDamage.EnableDamageArea();
        StartBurning();
    }

    public void Ignite()
    {
        if (!isIgnited && !isOnFire && !isBurned)
        {
            isIgnited = true;
            StartCoroutine(IgniteWithDelay());
        }
    }

    private IEnumerator IgniteWithDelay()
    {
        yield return new WaitForSeconds(ignitionDelay);
        StartBurning();
    }

    private void StartBurning()
    {
        if (isOnFire || isBurned) return;

        isIgnited = false;
        isOnFire = true;
        GetComponent<Collider>().enabled = false;
        fireEffect.gameObject.SetActive(true);
        fireLight.DOIntensity(2.5f, 1f);

        SpreadFire();
        StartCoroutine(BurnOut());
    }

    private void SpreadFire()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, spreadRadius);
        foreach (Collider col in nearbyColliders)
        {
            GrassBurning nearbyGrass = col.GetComponent<GrassBurning>();
            if (nearbyGrass != null && !nearbyGrass.IsOnFire() && !nearbyGrass.IsIgnited() && !nearbyGrass.IsBurned())
            {
                nearbyGrass.Ignite();
            }
        }
    }

    private IEnumerator BurnOut()
    {
        float duration = burnDuration/2;
        yield return new WaitForSeconds(duration);
        GrassPrefab.transform.DOScaleY(0.2f, burnDuration);
        yield return new WaitForSeconds(burnDuration);
        isOnFire = false;
        isBurned = true;

        // Replace with burned grass
        if (burnedGrassPrefab != null)
        {
            burnedGrassPrefab.SetActive(true);
        }

        fireEffect.gameObject.SetActive(false);

        GrassPrefab.SetActive(false);

        fireDamage.damageActive = false;

        Die();
    }

    public bool IsIgnited()
    {
        return isIgnited;
    }

    public bool IsOnFire()
    {
        return isOnFire;
    }

    public bool IsBurned()
    {
        return isBurned;
    }
}
