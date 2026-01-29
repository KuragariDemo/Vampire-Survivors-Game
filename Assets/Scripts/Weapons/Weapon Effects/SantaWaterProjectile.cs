using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles behavior for a water projectile fired by Santa character
public class SantaWaterProjectile : Projectile
{
    public float maxDistanceFromPlayer = 5f;

    private Transform playerTransform;
    private Vector3 targetPosition;

    // Initialize projectile and get reference to player
    protected override void Start()
    {
        base.Start();
        playerTransform = owner.transform;
    }

    // Update projectile behavior each physics frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Restrict projectile within a max distance from the player
        float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceFromPlayer > maxDistanceFromPlayer)
        {
            Vector3 direction = (transform.position - playerTransform.position).normalized;
            transform.position = playerTransform.position + direction * maxDistanceFromPlayer;
        }

        // Move projectile toward its target
        MoveTowardsTarget();
    }

    // Moves the projectile toward its target position
    private void MoveTowardsTarget()
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        transform.position += directionToTarget * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
        }
    }
}
