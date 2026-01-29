using TMPro;
using UnityEngine;

/// <summary>
/// Component that is attached to GameObjects to make it display the player's coins.
/// Either in-game, or the total number of coins the player has, depending on whether
/// the collector variable is set.
/// </summary>
public class UICoinDisplay : MonoBehaviour
{
    TextMeshProUGUI displayTarget;
    public PlayerCollector collector;

    void Start()
    {
        displayTarget = GetComponentInChildren<TextMeshProUGUI>();
        UpdateDisplay();
        if(collector != null) collector.onCoinCollected += UpdateDisplay;
    }

    private void Reset()
    {
        collector = FindObjectOfType<PlayerCollector>();
    }

    public void UpdateDisplay()
    {
        // If a collector is assigned, we will display the number of coins the collector has.
        if (collector != null)
        {
            displayTarget.text = Mathf.RoundToInt(collector.GetCoins()).ToString();
        }
        else
        {
            // If not, we will get the current number of coins that are saved.
            float coins = SaveManager.LastLoadedGameData.coins;
            displayTarget.text = Mathf.RoundToInt(coins).ToString();
        }
    }
}