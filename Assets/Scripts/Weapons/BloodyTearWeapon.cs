using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyTearWeapon : WhipWeapon
{
    private float lastHealTime = 0f;
    private float healCooldown = 1f;

    public override float GetDamage()
    {
        float baseDamage = currentStats.GetDamage() * owner.Stats.might;

        
        float critChance = 0.1f * owner.Stats.luck; //crit chance scales by luck
        bool isCritical = Random.value < critChance;

        if (isCritical)
        {
            baseDamage *= 2f; // Critical hits deal 2x damage
            Debug.Log("crit!");

            //Apply lifesteal on critical hit with a 1 second cooldown
            if (Time.time >= lastHealTime + healCooldown)
            {
                owner.CurrentHealth += 8;
                lastHealTime = Time.time;
                Debug.Log(owner.CurrentHealth);
            }
        }

        return baseDamage;
    }
}
