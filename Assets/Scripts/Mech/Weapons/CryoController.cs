using UnityEngine;
using FORGE3D;

public class CryoController : MechWeapon
{
    [Header("Cryo gun")]
    public GameObject cryoProjectiles;
    private float shotTimer;
    public float stunTime;
    public ProjectileWeapon projectileWeapon;

    public override void Init()
    {
        base.Init();
        weaponType = WeaponType.Cryo;
        weaponFuelManager.constantUse = false;
    }

    public void Update()
    {
        var target = sensor.GetNearestDetection();
        Vector3 location = transform.forward;
        if (target != null)
        {
            var hunter = target.GetComponent<CrawlerHunter>();
            if(hunter != null)
            {
                if (hunter.isStealthed)
                {
                    location = transform.forward;
                }
                else
                {
                    location = target.transform.position - gunturret.transform.position + aimOffest;
                }
            }
            else
            {
                location = target.transform.position - gunturret.transform.position + aimOffest;
            }

        }
        else
        {
            location = transform.forward;
        }


        gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, location, Time.deltaTime * autoAimSpeed);

        shotTimer += Time.deltaTime;

        if (weaponFuelManager.weaponFuel - weaponFuelUseRate <= 0)
        {
            Stop();
            return;
        }

        if (!isFiring)
        {
            return;
        }

        if (shotTimer >= fireRate)
        {
            shotTimer = 0;
            projectileWeapon.Cryo(damage, force, stunTime);
            weaponFuelManager.UseFuel(weaponFuelManager.weaponFuelRate);
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shotTimer = fireRate;
    }

}
