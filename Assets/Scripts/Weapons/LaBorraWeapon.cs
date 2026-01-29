using System.Collections;
using UnityEngine;

public class LaBorraWeapon : SantaWaterWeapon
{
    protected override void OnProjectileSpawned(Projectile projectile)
    {
        if (projectile.owner == owner)
        {
            float auraArea = currentStats.area * owner.Stats.area;
            StartCoroutine(SpawnAuraAfterLifespan(projectile, 1.2f, auraArea));
        }
    }
    public virtual IEnumerator SpawnAuraAfterLifespan(Projectile proj, float delay, float auraArea)
    {
        yield return new WaitForSeconds(delay);

        // Destroy the projectile after its lifespan expires
        Destroy(proj.gameObject);

        if (proj != null)
        {
            Vector2 pos = proj.transform.position;

            // Instantiate the aura
            Aura aura = Instantiate(currentStats.auraPrefab, pos, Quaternion.identity);
            aura.weapon = this;
            aura.owner = owner;

            // Set the size of the aura
            aura.transform.localScale = new Vector3(auraArea, auraArea, auraArea);

            // Timer-based movement towards the player
            float timeToReachPlayer = GetStats().lifespan;  // Time to reach player equals the aura's lifespan
            Vector2 playerPos = owner.transform.position; // Position of the player
            float elapsedTime = 0f;  // Timer to track the movement duration

            // Move the aura over its lifespan duration
            while (elapsedTime < timeToReachPlayer)
            {
                // Move the aura closer to the player based on the elapsed time
                float distanceToCover = GetStats().speed/20 * Time.deltaTime;
                aura.transform.position = Vector2.MoveTowards(aura.transform.position, playerPos, distanceToCover);

                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;  // Wait for the next frame
            }

            // Ensure the aura ends exactly at the player position
            aura.transform.position = playerPos;

            // Destroy the aura after its lifespan expires
            Destroy(aura.gameObject, GetStats().lifespan);
        }

    }


}
