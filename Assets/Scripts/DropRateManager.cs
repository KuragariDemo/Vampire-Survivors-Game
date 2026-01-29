using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]   //Serialize the class
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
        [Range(0, 1)] public float luckScaling = 1;
    }
    public bool active = false;
    public List<Drops> drops;

    void OnDestroy()
    {
        if (!active) return;
        if (!gameObject.scene.isLoaded) //Stops the spawning error from appearing when stopping play mode
        {
            return;
        }
        float playerluck = FindObjectOfType<PlayerStats>().Actual.luck;
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            float effectiveChance = Mathf.Min(rate.dropRate * playerluck, 100f);
            if (randomNumber <= rate.dropRate * (1 + (playerluck - 1f) * rate.luckScaling))
            {
                possibleDrops.Add(rate);
            }
        }
        //Check if there are possible drops
        if (possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}