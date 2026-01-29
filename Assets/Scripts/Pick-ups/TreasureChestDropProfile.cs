using UnityEngine;

[CreateAssetMenu(fileName = "Treasure Chest Drop Profile", menuName = "2D Top-down Rogue-like/Treasure Chest Drop Profile")]
public class TreasureChestDropProfile : ScriptableObject
{
    [Header("General Settings")]
    public string profileName = "Drop Profile";
    [Range(0, 1)] public float luckScaling = 0;
    [Range(0, 100)] public float baseDropChance;
    public float animDuration;

    [Header("Fireworks")]
    public bool hasFireworks = false;
    [Range(0f, 100f)] public float fireworksDelay = 1f;

    [Header("Item Display Settings")]
    public int noOfItems = 1;
    public Color[] beamColors = new Color[] { new Color(0, 0, 1, 0.6f) };

    [Range(0f, 100f)] public float delayTime = 0f;
    public int delayedBeams = 0;

    public bool hasCurvedBeams;
    public float curveBeamsSpawnTime;

    [Header("Coins")]
    public float maxCoins = 0;
    public float minCoins = 0;

    [Header("Chest Sound Effects")]
    public AudioClip openingSound;
}
