using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleChallenge : MonoBehaviour
{
    [HideInInspector] public ObstacleSpawner spawner;

    void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag("Player"))
        {
            Debug.Log("[ObstacleChallenge] You hit the pillar!");
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            spawner.OnPillarComplete(true);
        }
    }
}
