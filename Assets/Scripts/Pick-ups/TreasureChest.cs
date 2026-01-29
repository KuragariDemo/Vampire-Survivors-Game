using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [System.Flags]
    public enum DropType
    {
        NewPassive = 1, NewWeapon = 2, UpgradePassive = 4,
        UpgradeWeapon = 8, Evolution = 16
    }
    public DropType possibleDrops = (DropType)~0;

    public enum DropCountType { sequential, random }
    public DropCountType dropCountType = DropCountType.sequential;
    public TreasureChestDropProfile[] dropProfiles;
    public static int totalPickups = 0;
    int currentDropProfileIndex = 0;

    PlayerInventory recipient;

    private int GetRewardCount()
    {
        TreasureChestDropProfile dp = GetNextDropProfile();
        if(dp) return dp.noOfItems;
        return 1;
    }

    public TreasureChestDropProfile GetCurrentDropProfile() 
    {
        return dropProfiles[currentDropProfileIndex];
    }

    public TreasureChestDropProfile GetNextDropProfile()
    {
        if (dropProfiles == null || dropProfiles.Length == 0)
        {
            Debug.LogWarning("Drop profiles not set.");
            return null;
        }

        switch (dropCountType)
        {
            case DropCountType.sequential:
                currentDropProfileIndex = Mathf.Clamp(
                    totalPickups, 0,
                    dropProfiles.Length - 1
                );
                break;

            case DropCountType.random:

                float playerLuck = recipient.GetComponentInChildren<PlayerStats>().Actual.luck;

                List<(int index, TreasureChestDropProfile profile, float weight)> weightedProfiles = new List<(int, TreasureChestDropProfile, float)>();
                for (int i = 0; i < dropProfiles.Length; i++)
                {
                    float weight = dropProfiles[i].baseDropChance * (1 + dropProfiles[i].luckScaling * (playerLuck - 1));
                    weightedProfiles.Add((i, dropProfiles[i], weight));
                }

                weightedProfiles.Sort((a, b) => a.weight.CompareTo(b.weight));

                float totalWeight = 0f;
                foreach (var entry in weightedProfiles)
                    totalWeight += entry.weight;

                float r = Random.Range(0, totalWeight);
                float cumulative = 0f;
                foreach (var entry in weightedProfiles)
                {
                    cumulative += entry.weight;
                    if (r <= cumulative)
                    {
                        currentDropProfileIndex = entry.index;
                        return entry.profile;
                    }
                }
                break;
        }

        return GetCurrentDropProfile();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out PlayerInventory p))
        {
            recipient = p;

            int rewardCount = GetRewardCount();
            for (int i = 0; i < rewardCount; i++)
            {
                Open(p);
            }
            gameObject.SetActive(false);

            UITreasureChest.Activate(p.GetComponentInChildren<PlayerCollector>(), this);

            totalPickups = (totalPickups + 1) % (dropProfiles.Length + 1);
        }
    }

    void Open(PlayerInventory inventory)
    {
        if (inventory == null) return;

        if (possibleDrops.HasFlag(DropType.Evolution) && TryEvolve<Weapon>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.UpgradeWeapon) && TryUpgrade<Weapon>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.UpgradePassive) && TryUpgrade<Passive>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.NewWeapon) && TryGive<WeaponData>(inventory, false)) return;
        if (possibleDrops.HasFlag(DropType.NewPassive)) TryGive<PassiveData>(inventory, false);
    }

    public void NotifyComplete() {
        recipient.weaponUI.Refresh();
        recipient.passiveUI.Refresh();
    }

    T TryEvolve<T>(PlayerInventory inventory, bool updateUI = true) where T : Item
    {
        T[] evolvables = inventory.GetEvolvables<T>();
        foreach (Item i in evolvables)
        {
            ItemData.Evolution[] possibleEvolutions = i.CanEvolve(0);
            foreach (ItemData.Evolution e in possibleEvolutions)
            {
                if (i.AttemptEvolution(e, 0, updateUI))
                {
                    UITreasureChest.NotifyItemReceived(e.outcome.itemType.icon);
                    return i as T;
                }
            }
        }
        return null;
    }

    T TryUpgrade<T>(PlayerInventory inventory, bool updateUI = true) where T : Item
    {
        T[] upgradables = inventory.GetUpgradables<T>();
        if (upgradables.Length == 0) return null; 

        T t = upgradables[Random.Range(0, upgradables.Length)];
        inventory.LevelUp(t, updateUI);
        UITreasureChest.NotifyItemReceived(t.data.icon);
        return t;
    }

    T TryGive<T>(PlayerInventory inventory, bool updateUI = true) where T : ItemData
    {
        T[] possibilities = inventory.GetUnowned<T>();
        if (possibilities.Length == 0) return null;

        T t = possibilities[Random.Range(0, possibilities.Length)];
        inventory.Add(t, updateUI);
        UITreasureChest.NotifyItemReceived(t.icon);
        return t;
    }
}