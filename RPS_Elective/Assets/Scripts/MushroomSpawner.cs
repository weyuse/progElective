using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MushroomSpawner : MonoBehaviour
{
    public GameObject mushroomPrefab;

    // Interval between spawns in seconds
    public float spawnInterval = 5f; 
    private float timer = 20f;
    //Taking the arena size from the pirate controller
    private PirateShipController shipController;

    void Start()
    {
        shipController = FindObjectOfType<PirateShipController>();
        timer = spawnInterval;
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 minBounds, Vector3 maxBounds)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(minBounds.x, maxBounds.x),
                0f,
                Random.Range(minBounds.z, maxBounds.z)
            );

            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out UnityEngine.AI.NavMeshHit hit, 100f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find a valid point on the NavMesh.");
        return transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        // counting till the set time
        timer += Time.deltaTime;

        // waiting for timer to reach the required amount of seconds
        if (timer >= spawnInterval)
        {
            // reset the timer now that somethings spawning
            timer = 0f;

            // generate random spawn position within the arena with a 3d vector
            Vector3 randomPoint = GetRandomPointOnNavMesh(shipController.mapMinBounds, shipController.mapMaxBounds);


            // instantiate the mushroom at the random position - quaternion meaning keeping the rotation as the prefab dictates
            Instantiate(mushroomPrefab, randomPoint, Quaternion.identity);

            Debug.Log("Mushroom");
        }
    }
}
