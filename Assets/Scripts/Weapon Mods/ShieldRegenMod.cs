using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRegenMod : WeaponMod
{
    public float regenAmount = 0f;
    public float regenCooldown = 0.2f;
    private float lastRegenTime;
    private MechHealth mechHealth;
    private bool _enabled = false;

    public override void Init()
    {
        base.Init();
        mechHealth = BattleMech.instance.mechHealth;
        lastRegenTime = Time.time - regenCooldown; // Allow immediate regen on first update
        regenAmount = runMod.modifiers[0].statValue; // Assuming the first modifier is the regen amount
        _enabled = true;
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        mechHealth.altShieldActive = false; // Ensure shield is inactive when mods are removed
        _enabled = false; // Disable the mod
    }

    public void ApplyRegen()
    {
        if (Time.time - lastRegenTime >= regenCooldown)
        {
            mechHealth.Heal(regenAmount);
            lastRegenTime = Time.time;
        }
    }

    void Update()
    {
        if (!_enabled)
        {
            return; // Exit if mod is not enabled or shield is not active
        }
        if (mechHealth.altShieldActive)
        {
            ApplyRegen();
        }
    }

}
