using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSpiralWeapon : ProjectileWeapon
{
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning($"Projectile prefab has not been set for {name}");
            ActivateCooldown(true);
            return false;
        }

        if (!CanAttack()) return false;

        // Spawn 9 projectiles evenly spaced around the player (360° / 9 = 40° apart)
        int projectileCount = 9;
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float spawnAngle = i * angleStep; // Rotate evenly

            // Spawn projectile
            Projectile prefab = Instantiate(
                currentStats.projectilePrefab,
                owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
                Quaternion.Euler(0, 0, spawnAngle)
            );

            prefab.weapon = this;
            prefab.owner = owner;
        }

        ActivateCooldown(true);
        return true;
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        // Offset from player in a circle shape
        float radius = 1.5f; // Adjust distance from player
        return new Vector2(
            Mathf.Cos(spawnAngle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(spawnAngle * Mathf.Deg2Rad) * radius
        );
    }
}
