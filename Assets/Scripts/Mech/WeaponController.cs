using UnityEngine;
using System.Collections;
using System;
using FORGE3D;


public class WeaponController : MonoBehaviour
{
    // Singleton instance
    public static WeaponController instance;

    // Current firing socket
    private int curSocket = 0;

    [Header("Turret setup")] public Transform[] TurretSocket; // Sockets reference
    public ParticleSystem[] ShellParticles; // Bullet shells particle system

    [Header("Vulcan")] public Transform vulcanProjectile; // Projectile prefab
    public Transform vulcanMuzzle; // Muzzle flash prefab  
    public Transform vulcanImpact; // Impact prefab
    public float vulcanOffset;
    public int vulcanDamage;

    private void Awake()
    {
        // Initialize singleton  
        instance = this;

            // Initialize bullet shells particles
            for (int i = 0; i < ShellParticles.Length; i++)
            {
                var em = ShellParticles[i].emission;
                em.enabled = false;
                ShellParticles[i].Stop();
                ShellParticles[i].gameObject.SetActive(true);
            }
    }

    // Advance to next turret socket
    private void AdvanceSocket()
    {
        curSocket++;
        if (curSocket >= TurretSocket.Length)
            curSocket = 0;
    }

    // Fire turret weapon
    public void Fire(int dam)
    {
        vulcanDamage = dam;
        Vulcan();
    }

    // Stop firing 
    public void Stop()
    {

    }

    // Fire vulcan weapon
    private void Vulcan()
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
            proj.impactDamage = vulcanDamage;
        }

        // Emit one bullet shell
        if (ShellParticles.Length > 0)
            ShellParticles[curSocket].Emit(1);

        // Play shot sound effect
        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        // Advance to next turret socket
        AdvanceSocket();
    }

    // Spawn vulcan weapon impact
    public void VulcanImpact(Vector3 pos)
    {
        // Spawn impact prefab at specified position
        F3DPoolManager.Pools["GeneratedPool"].Spawn(vulcanImpact, pos, Quaternion.identity, null);
        // Play impact sound effect
        F3DAudioController.instance.VulcanHit(pos);
    }
}
