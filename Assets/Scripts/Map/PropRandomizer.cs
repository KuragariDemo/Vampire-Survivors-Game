using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints = new List<GameObject>();
    public List<GameObject> propPrefabs = new List<GameObject>();

    void Start()
    {
        if (propPrefabs.Count > 0)
        {
            SpawnProps();
        }
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            if (sp == null) continue;

            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }
}
