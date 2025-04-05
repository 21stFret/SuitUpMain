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

    public float range;

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

    public void Shotgun(float dam, float force, int index, float angle, int burst, int acutalI, float stunTime, bool shockRounds, float shockDamage)
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
            //proj.bounceCount = 0;
            //proj.pierceCount = 0;
            proj.shockRounds = shockRounds;
            proj.shockDamage = shockDamage;
            proj.range = range;
        }

        F3DAudioController.instance.ShotGunShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Cryo(float dam, float force, float stunTime, int shards = 1)
    {
        float angle = 15f; // Angle between each shard
        int startigIndex = shards>0? -shards : 0;
        for (int i = startigIndex; i <= shards; i++)
        {
            angle = i < 0 ? -angle : angle;
            float yRotation = angle * i; // Change the rotation of the y axis based on the loop index
            var newGO =
            F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanProjectile,
            TurretSocket[curSocket].position,
            TurretSocket[curSocket].rotation * Quaternion.Euler(0f, yRotation, 0f)).gameObject; // Apply y rotation

            var proj = newGO.gameObject.GetComponent<F3DProjectile>();
            if (proj)
            {
                proj.impactDamage = dam;
                proj.impactForce = force;
                proj._weaponController = this;
                proj.stunTime = stunTime;
                proj.weaponType = WeaponType.Cryo;
            }
        }

        F3DAudioController.instance.SniperShot(TurretSocket[curSocket].position);
        AdvanceSocket();
    }

    public void Minigun(float dam, int bounce)
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
            proj.impactForce = 1;
            proj.bounceCount = bounce;
        }


        // Emit one bullet shell
        if (ShellParticles.Length > 0)
            ShellParticles[curSocket].Emit(1);

        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Laser(float dam, int pierceC, bool splitrounds = false, int splitCount =1)
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
            proj.weaponType = WeaponType.Plasma;
            proj.pierceCount = pierceC;
            proj.pierceCountMax = pierceC;
            proj.splitRounds = splitrounds;
            proj.splitCount = splitCount;
        }


        // Emit one bullet shell
        if (ShellParticles.Length > 0)
            ShellParticles[curSocket].Emit(1);

        F3DAudioController.instance.PlasmaGunShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    public void Impact(Vector3 pos, WeaponType type)
    {
        Debug.DrawLine(TurretSocket[curSocket].position, pos, Color.red, 2f);
        // Spawn impact prefab at specified position
        F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanImpact, pos, Quaternion.identity, null);

        switch (type)
        {
            case WeaponType.Minigun:
                F3DAudioController.instance.VulcanHit(pos);
                break;
            case WeaponType.Shotgun:
                F3DAudioController.instance.ShotGunHit(pos);
                break;
            case WeaponType.Cryo:
                F3DAudioController.instance.SniperHit(pos);
                break;
            case WeaponType.Plasma:
                F3DAudioController.instance.PlasmaGunHit(pos);
                break;
        }
    }
}
