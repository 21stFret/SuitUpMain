using UnityEngine;
using System.Collections;
using System;
using FORGE3D;


public class ProjectileWeapon : MonoBehaviour
{
    private int curSocket = 0;

    [Header("Turret setup")] 
    public Transform[] TurretSocket;
    public ParticleSystem[] ShellParticles;

    [Header("Vulcan")] 
    public Transform vulcanProjectile;
    public Transform vulcanMuzzle; 
    public Transform vulcanImpact; 

    private void Awake()
    {
        for (int i = 0; i < ShellParticles.Length; i++)
        {
            var em = ShellParticles[i].emission;
            em.enabled = false;
            ShellParticles[i].Stop();
            ShellParticles[i].gameObject.SetActive(true);
        }
    }

    private void AdvanceSocket()
    {
        curSocket++;
        if (curSocket >= TurretSocket.Length)
            curSocket = 0;
    }

    public void Shotgun(float dam, float force, int index, float angle, int burst, int acutalI, float stunTime)
    {
        float rand = UnityEngine.Random.Range(-3, 3);
        float Angle = ((angle / burst)) + rand;
        if(index<0)
        {
            Angle = -Angle;
        }
        // Spawn muzzle flash and projectile at current socket position
        F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanMuzzle, TurretSocket[curSocket].position,
            TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
        var newGO =
            F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile,
                TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation * Quaternion.Euler(0f, Angle * acutalI, 0f), null).gameObject;

        var proj = newGO.gameObject.GetComponent<F3DProjectile>();
        if (proj)
        {
            proj.impactDamage = dam;
            proj.impactForce = force;
            proj._weaponController = this;
            proj.stunTime = stunTime;
            proj.weaponType = WeaponType.Shotgun;
        }

        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Cryo(float dam, float force, float stunTime)
    {
        var newGO =
            F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile,
                TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation).gameObject;

        var proj = newGO.gameObject.GetComponent<F3DProjectile>();
        if (proj)
        {
            proj.impactDamage = dam;
            proj.impactForce = force;
            proj._weaponController = this;
            proj.stunTime = stunTime;
            proj.weaponType = WeaponType.Cryo;
        }

        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Minigun(float dam)
    {
        // Spawn muzzle flash and projectile at current socket position
        F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanMuzzle, TurretSocket[curSocket].position,
            TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
        var newGO =
            F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile,
                TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, null).gameObject;

        var proj = newGO.gameObject.GetComponent<F3DProjectile>();
        if (proj)
        {
            proj.impactDamage = dam;
            proj._weaponController = this;
            proj.weaponType = WeaponType.Minigun;
        }


        // Emit one bullet shell
        if (ShellParticles.Length > 0)
            ShellParticles[curSocket].Emit(1);

        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Impact(Vector3 pos)
    {
        Debug.DrawLine(TurretSocket[curSocket].position, pos, Color.red, 2f);
        // Spawn impact prefab at specified position
        F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanImpact, pos, Quaternion.identity, null);
        F3DAudioController.instance.VulcanHit(pos);
    }
}
