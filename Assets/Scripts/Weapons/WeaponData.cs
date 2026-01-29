using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        // Pick the stats from the next level.
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        // Otherwise, pick one of the stats from the random growth array.
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        // Return an empty value and a warning.
        Debug.LogWarning(string.Format("Weapon doesn't have its level up stats configured for Level {0}!",level));
        return new Weapon.Stats();
    }

}
