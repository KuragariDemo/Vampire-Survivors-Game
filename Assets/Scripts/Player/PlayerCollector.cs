using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D detector;
    public float pullSpeed = 10;

    public delegate void OnCoinCollected();
    public OnCoinCollected onCoinCollected;

    float coins;

    void Start()
    {
        player = GetComponentInParent<PlayerStats>();
        coins = 0;
    }

    public void SetRadius(float r)
    {
        if (!detector) detector = GetComponent<CircleCollider2D>();
        detector.radius = r;
    }

    public float GetCoins() { return coins; }
    //Updates coins Display and information
    public float AddCoins(float amount)
    {
        coins += amount;
        onCoinCollected();
        return coins;
    }

    // Saves the collected coins to the save file.
    public void SaveCoinsToStash()
    {
        SaveManager.LastLoadedGameData.coins += coins;
        SaveManager.Save();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Pickup p))
        {
            p.Collect(player, pullSpeed);
        }
    }
}