using UnityEngine;

public class FlameController : MechWeapon
{
    [Header("Flame gun")]
    public FlameTrigger flameTrigger;

    public override void Init()
    {
        base.Init();
        weaponType = WeaponType.Flame;
        flameTrigger.InitFlameTrigger(damage, fireRate,range);
        var effect = weaponEffects.weaponEffect.main;
        effect.startSpeed = range * 2;
    }

    void Update()
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
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        flameTrigger.SetCol(isFiring);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        flameTrigger.SetCol(isFiring);
    }

}
