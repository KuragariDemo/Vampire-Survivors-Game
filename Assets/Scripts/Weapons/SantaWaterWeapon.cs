using System.Collections;
using UnityEngine;

// Custom weapon class for Santa's water projectile attack
public class SantaWaterWeapon : ProjectileWeapon
{


    // Triggered when a projectile is spawned; starts aura spawn coroutine
    protected override void OnProjectileSpawned(Projectile projectile)
    {
        if (projectile.owner == owner)
        {
            StartCoroutine(SpawnAuraAfterLifespan(projectile, 1.2f));
        }
    }

    // Spawns an aura at the projectile's position after a delay, then destroys both
    public virtual IEnumerator SpawnAuraAfterLifespan(Projectile proj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (proj != null)
        {
            Vector2 pos = proj.transform.position;

            Aura aura = Instantiate(currentStats.auraPrefab, pos, Quaternion.identity);
            aura.weapon = this;
            aura.owner = owner;

            float trueArea = base.GetArea();
            aura.transform.localScale = new Vector3(trueArea, trueArea, trueArea);

            Destroy(aura.gameObject, GetStats().lifespan);
            Destroy(proj.gameObject);
        }
    }

    // Override to prevent scaling of the bottle projectile
    public override float GetArea()
    {
        return 1;
    }

    // Calculates projectile spawn offset above the player’s position
    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        Vector2 playerPos = owner.transform.position;
        Vector2 topOfScreen = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1f));
        Vector2 spawnPos = new Vector2(playerPos.x, topOfScreen.y + 1f);
        return spawnPos - playerPos;
    }
}
