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
        weaponFuelManager.constantUse = false;
    }

    public void Update()
    {
        if (!isFiring)
        {
            return;
        }

        shotTimer += Time.deltaTime;
        if (shotTimer >= fireRate)
        {
            shotTimer = 0;
            projectileWeapon.Cryo(damage, force, stunTime);
            weaponFuelManager.weaponFuel -= weaponFuelManager.weaponFuelRate;
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shotTimer = 0;
    }

}
