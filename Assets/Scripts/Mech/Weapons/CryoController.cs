using UnityEngine;
using FORGE3D;

public class CryoController : MechWeapon
{
    [Header("Cryo gun")]
    public GameObject cryoProjectiles;
    private float shotTimer;
    public float stunTime;
    public ProjectileWeapon projectileWeapon;
    private bool _isFiring;

    public override void Init()
    {
        base.Init();
    }

    public void Update()
    {
        if (!isFiring)
        {
            return;
        }

        weaponFuelManager.uiEnabled = false;
        shotTimer += Time.deltaTime;
        if (shotTimer >= fireRate)
        {
            shotTimer = 0;
            projectileWeapon.Cryo(damage, force, stunTime);
            weaponFuelManager.uiEnabled = true;
        }
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();

    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shotTimer = 0;
        weaponFuelManager.uiEnabled = true;
    }

}
