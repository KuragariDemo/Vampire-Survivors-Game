using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkOptimizer : MonoBehaviour
{
    public float maxDist;
    float dist;
    public Transform player;

    void Start()
    {
        
    }

    void Update()
    {
        dist = Vector3.Distance(player.position, transform.position);
        if(dist > maxDist)
        {

        }
    }
}
