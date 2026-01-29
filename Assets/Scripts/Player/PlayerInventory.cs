using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
            }
            else
            {
                Passive p = item as Passive;
            }
            Debug.Log(string.Format("Assigned {0} to player.", item.name));
        }

        public void Clear()
        {
            item = null;
        }

        public bool IsEmpty() { return item == null; }
    }
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);
    public UIInventoryIconsDisplay weaponUI, passiveUI;

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();    //List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); //List of upgrade options for passive items

    public UIUpgradeWindow upgradeWindow;

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Checks if the inventory has an item of a certain type.
    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    // Find a passive of a certain type in the inventory.
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p && p.data == type)
                return p;
        }
        return null;
    }

    // Find a weapon of a certain type in the inventory.
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
                return w;
        }
        return null;
    }

    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Passive p = weaponSlots[i].item as Passive;
            if (p.data == data)
            {
                weaponSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }


    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }


    public int Add(WeaponData data, bool updateUI = true)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }


        if (slotNum < 0) return slotNum;

        
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(transform); //Set the weapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);
            if(updateUI) weaponUI.Refresh();

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format(
                "Invalid weapon type specified for {0}.",
                data.name
            ));
        }

        return -1;
    }

    public int Add(PassiveData data, bool updateUI = true)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); //Set the weapon to be a child of the player
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);
        if (updateUI) passiveUI.Refresh();

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }

    public int Add(ItemData data, bool updateUI = true)
    {
        if (data is WeaponData) return Add(data as WeaponData, updateUI);
        else if (data is PassiveData) return Add(data as PassiveData, updateUI);
        return -1;
    }

    public bool LevelUp(ItemData data, bool updateUI = true)
    {
        Item item = Get(data);
        if (item) return LevelUp(item, updateUI);
        return false;
    }

    public bool LevelUp(Item item, bool updateUI = true)
    {
        if (!item.DoLevelUp())
        {
            Debug.LogWarning(string.Format(
                "Failed to level up {0}.",
                 item.name
            ));
            return false;
        }

        if (updateUI)
        {
            weaponUI.Refresh();
            passiveUI.Refresh();
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        if (item is Passive) player.RecalculateStats();
        return true;
    }

    public Slot[] GetSlots<T>() where T : Item
    {
        switch (typeof(T).ToString())
        {
            case "Passive":
                return passiveSlots.ToArray();
            case "Weapon":
                return weaponSlots.ToArray();
            case "Item":
                List<Slot> s = new List<Slot>(passiveSlots);
                s.AddRange(weaponSlots);
                return s.ToArray();
        }

        Debug.LogWarning("Generic type provided to GetSlots() call does not have a coded behaviour.");
        return null;
    }

    public Slot[] GetSlotsFor<T>() where T : ItemData
    {
        if (typeof(T) == typeof(PassiveData))
        {
            return passiveSlots.ToArray();
        }
        else if (typeof(T) == typeof(WeaponData))
        {
            return weaponSlots.ToArray();
        }
        else if (typeof(T) == typeof(ItemData))
        {
            List<Slot> s = new List<Slot>(passiveSlots);
            s.AddRange(weaponSlots);
            return s.ToArray();
        }
        Debug.LogWarning("Generic type provided to GetSlotsFor() call does not have a coded behaviour.");
        return null;
    }


    int GetSlotsLeft(List<Slot> slots)
    {
        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    public int GetSlotsLeft<T>() where T : Item { return GetSlotsLeft(new List<Slot>( GetSlots<T>() )); }
    public int GetSlotsLeftFor<T>() where T : ItemData { return GetSlotsLeft(new List<Slot>( GetSlotsFor<T>() )); }

    public T[] GetAvailable<T>() where T : ItemData
    {
        if (typeof(T) == typeof(PassiveData))
        {
            return availablePassives.ToArray() as T[];
        }
        else if (typeof(T) == typeof(WeaponData))
        {
            return availableWeapons.ToArray() as T[];
           
        }
        else if (typeof(T) == typeof(ItemData))
        {
            List<ItemData> list = new List<ItemData>(availablePassives);
            list.AddRange(availableWeapons);
            return list.ToArray() as T[];
        }

        Debug.LogWarning("Generic type provided to GetAvailable() call does not have a coded behaviour.");
        return null;
    }

    public T[] GetUnowned<T>() where T : ItemData
    {
        
        var available = GetAvailable<T>();
        
        if (available == null || available.Length == 0)
            return new T[0]; 

        List<T> list = new List<T>(available);

        
        var slots = GetSlotsFor<T>();
        if (slots != null)
        {
            foreach (Slot s in slots)
            {
                if (s?.item?.data != null && list.Contains(s.item.data as T))
                    list.Remove(s.item.data as T);
            }
        }
        return list.ToArray();
    }

    public T[] GetEvolvables<T>() where T : Item
    {
        
        List<T> result = new List<T>();
        foreach (Slot s in GetSlots<T>())
            if (s.item is T t && t.CanEvolve(0).Length > 0) result.Add(t);
        return result.ToArray();
    }

    public T[] GetUpgradables<T>() where T : Item
    {
        
        List<T> result = new List<T>();
        foreach (Slot s in GetSlots<T>())
            if (s.item is T t && t.CanLevelUp()) result.Add(t);
        return result.ToArray();
    }

    void ApplyUpgradeOptions()
    {
  
        List<ItemData> availableUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        foreach (ItemData data in allUpgrades)
        {
            Item obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel) availableUpgrades.Add(data);
            }
            else
            {
                if (data is WeaponData && weaponSlotsLeft > 0) availableUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotsLeft > 0) availableUpgrades.Add(data);
            }
        }

        int availUpgradeCount = availableUpgrades.Count;
        if (availUpgradeCount > 0)
        {
            bool getExtraItem = 1f - 1f / player.Stats.luck > UnityEngine.Random.value;
            if (getExtraItem || availUpgradeCount < 4) upgradeWindow.SetUpgrades(this, availableUpgrades, 4);
            else upgradeWindow.SetUpgrades(this, availableUpgrades, 3, "Increase your Luck stat for a chance to get 4 items!");
        }
        else if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void RemoveAndApplyUpgrades()
    {

        ApplyUpgradeOptions();
    }

}