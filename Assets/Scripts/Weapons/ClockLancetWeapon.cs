using UnityEngine;

public class ClockLancetWeapon : ProjectileWeapon
{
    public const int NUMBER_OF_ANGLES = 12;
    protected float currentAngle = 90; // -90 degrees points in the 12 o'clock direction.

    // How many degrees this weapon turns after every shot.
    protected static float turnAngle = -360f / NUMBER_OF_ANGLES;

    protected override bool Attack(int attackCount = 1)
    {
        // If the attack is successful, advance the current angle.
        if(base.Attack(1))
        {
            currentAngle += turnAngle;

            // If our result's value is more than 180 or less than -180.
            if(Mathf.Abs(currentAngle) > 180f)
	            // Convert the value to be between -180 and 180.
	            currentAngle = -Mathf.Sign(currentAngle) * (360f - Mathf.Abs(currentAngle));

            return true;
        }
        return false;
    }

    // Override the spawn direction of the weapon to shoot the
    // projectile in the current angle.
    protected override float GetSpawnAngle() { return currentAngle; }
}
