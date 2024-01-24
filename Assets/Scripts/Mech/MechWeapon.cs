using UnityEngine;

public class MechWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    public int[] _damage;
    [SerializeField]
    private float[] _fireRate;
    [SerializeField]
    private float[] _range;
    public int damage;
    public float fireRate;
    public float range;
    public int ammo;
    public int maxAmmo;
    public float reloadTime;
    public bool isFiring;

    public AudioSource weaponAudioSource;
    public AudioClip weaponClose;
    public AudioClip weaponLoop;

    public ParticleSystem weaponEffect;
    public GameObject weaponLights;

    public WeaponUI weaponUI;
    public float weaponFuel;
    public float weaponFuelMax = 100;
    public float weaponRechargeRate;
    public float weaponUseRate;
    public Sprite fuelSprite;

    public virtual void Init()
    {
        weaponFuel = weaponFuelMax;
        SetValues();

        if (weaponUI == null)
        {
            return;
        }

        weaponUI.UpdateWeaponUI(weaponFuel);

    }

    private void SetValues()
    {
        print("Setting values for " + name);
        damage = _damage[weaponData.level];
        fireRate = _fireRate[weaponData.level];
        range = _range[weaponData.level];
    }

    private void Update()
    {
        FuelManagement();
    }

    public virtual void Fire()
    {
        //Debug.Log("Firing " + name);
        if (weaponEffect != null)
        {
            weaponEffect.Play();
        }
        weaponAudioSource.clip = weaponLoop;
        weaponAudioSource.loop = true;
        weaponAudioSource.Play();
        weaponLights.SetActive(true);
        weaponUI.SetUITrigger(false);
    }

    public virtual void Stop()
    {
        //Debug.Log("Stopping " + name);
        if (weaponEffect != null)
        {
            weaponEffect.Stop();
        }
        weaponAudioSource.Stop();
        weaponAudioSource.clip = weaponClose;
        weaponAudioSource.loop = false;
        weaponAudioSource.Play();
        weaponLights.SetActive(false);
        weaponUI.SetUITrigger(true);
    }

    private void FuelManagement()
    {
        if (isFiring)
        {
            weaponFuel -= weaponUseRate;

            if (weaponFuel <= 0)
            {
                Stop();
            }
        }

        if (weaponFuel >= weaponFuelMax)
        { return; }

        weaponFuel += weaponRechargeRate;

        if (weaponUI == null)
        {
            return;
        }

        weaponUI.UpdateWeaponUI(weaponFuel);

    }
}
