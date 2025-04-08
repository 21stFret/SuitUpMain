using Micosmo.SensorToolkit;
using System;
using UnityEngine;

public enum WeaponType
{
    Minigun,
    Shotgun,
    Plasma,
    Flame,
    Lightning,
    Cryo,
    AoE,
    Cralwer,
    Spore,
    Default,
    Trap,
    Heal
}

[Serializable]
public struct BaseWeaponInfo
{
    public string weaponName;
    public float[] _damage;
    public float[] _fireRate;
    public float[] _range;
    public float[] _weaponFuelUseRate;
    public float[] _uniqueValue;
    public int[] _cost;
    public int _unlockCost;
    public string weaponDescription;
}

[Serializable]
public struct WeaponEffects
{
    public AudioSource weaponAudioSource;
    public AudioClip weaponClose;
    public AudioClip weaponLoop;

    public ParticleSystem weaponEffect;
    public GameObject weaponLights;
}

public class MechWeapon : MonoBehaviour
{
    public WeaponFuelManager weaponFuelManager;
    public WeaponData weaponData;
    public Sprite weaponSprite;
    public BaseWeaponInfo baseWeaponInfo;
    public WeaponEffects weaponEffects;
    public WeaponType weaponType;
    public GameObject gunturret;
    public float damage;
    public float force;
    public float fireRate;
    public float range;
    public float weaponRechargeRate;
    public float weaponFuelUseRate;
    [HideInInspector]
    public bool isFiring;
    public RangeSensor rangeSensor;
    [Header("Main Weapon")]
    public LOSSensor sensor;
    public Vector3 aimOffest;
    public LaserSight laserSight;
    [Header("Weapon Mods")]
    public WeaponMod weaponMod;
    public bool weaponOverride;
    public int bounces;
    protected float autoAimSpeed = 2.0f;

    public virtual void Init()
    {
        SetValues();

        if(weaponData.mainWeapon)
        {
            rangeSensor = GetComponent<RangeSensor>();
            rangeSensor.Sphere.Radius = range;
            sensor.enabled = true;
            //laserSight.gameObject.SetActive(true);
            //laserSight.SetLaserLength(range);
        }
        else
        {
            weaponFuelManager.Init(this);
        }
    }

    public void ApplyDamageModifier(RunMod mod)
    {
        GameManager.instance.runUpgradeManager.ApplyMod(mod);
    }

    public void RemoveDamageModifier(RunMod mod)
    {
        GameManager.instance.runUpgradeManager.RemoveMod(mod);
    }

    private void SetValues()
    {
        //print("Setting values for " + name);
        damage = baseWeaponInfo._damage[weaponData.level];
        fireRate = baseWeaponInfo._fireRate[weaponData.level];
        range = baseWeaponInfo._range[weaponData.level];
        weaponFuelUseRate = baseWeaponInfo._weaponFuelUseRate[weaponData.level];
        force = baseWeaponInfo._uniqueValue[weaponData.level];
    }

    public void FireMod()
    {
        if(weaponMod == null)
        {
            //print("No weapon mod");
            return;
        }
        weaponMod.Fire();
    }

    public void StopMod()
    {
        if (weaponMod == null)
        {
            //print("No weapon mod");
            return;
        }  
        weaponMod.Stop();
    }

    public virtual void Fire()
    {
        if(!weaponData.mainWeapon)
        {
            weaponFuelManager.weaponInUse = true;
        }

        if (weaponMod != null && weaponOverride)
        {
            FireMod();
            return;
        }

        isFiring = true;

        if (weaponEffects.weaponEffect != null)
        {
            weaponEffects.weaponEffect.Play();
        }

        if (weaponEffects.weaponAudioSource != null)
        {
            weaponEffects.weaponAudioSource.clip = weaponEffects.weaponLoop;
            weaponEffects.weaponAudioSource.loop = true;
            weaponEffects.weaponAudioSource.Play();
        }

        if (weaponEffects.weaponLights !=null)
        {
            weaponEffects.weaponLights.SetActive(true);
        }


        if (weaponMod != null)
        {
            FireMod();
        }
    }

    public virtual void Stop()
    {
        if (!weaponData.mainWeapon)
        {
            weaponFuelManager.weaponInUse = false;
        }
        if (weaponMod != null && weaponOverride)
        {
            StopMod();
            return;
        }
        if (!isFiring)
        {
            return;
        }
        isFiring = false;

        if (weaponEffects.weaponEffect != null)
        {
            weaponEffects.weaponEffect.Stop();
        }

        if (weaponEffects.weaponAudioSource != null)
        {
            weaponEffects.weaponAudioSource.Stop();
            weaponEffects.weaponAudioSource.clip = weaponEffects.weaponClose;
            weaponEffects.weaponAudioSource.loop = false;
            weaponEffects.weaponAudioSource.Play();
        }

        if (weaponEffects.weaponLights != null)
        {
            weaponEffects.weaponLights.SetActive(false);
        }

        if (weaponMod != null)
        {
            StopMod();
        }
    }

}
