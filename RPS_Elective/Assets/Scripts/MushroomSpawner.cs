using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MushroomSpawner : MonoBehaviour
{
    public GameObject mushroomPrefab;
    private float spawnInterval = 5f; 
    private float timer = 0f;
    private PirateShipController shipController;
    private List<GameObject> mushrooms = new List<GameObject>();
    private int maxMushrooms = 4;

    void Start()
    {
        shipController = FindObjectOfType<PirateShipController>();
        if (shipController == null)
        {
            Debug.LogError("PirateShipController not found in the scene.");
            return;
        }
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

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find a valid point on the NavMesh.");
        return transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        mushrooms.RemoveAll(mushroom => mushroom == null);

        if (timer >= spawnInterval)
        {
            timer = 0f;

            if (mushrooms.Count < maxMushrooms)
            {
                Vector3 randomPoint = GetRandomPointOnNavMesh(shipController.mapMinBounds, shipController.mapMaxBounds);
                GameObject mushroom = Instantiate(mushroomPrefab, randomPoint, Quaternion.identity);
                mushrooms.Add(mushroom);
            }

            if (mushrooms.Count >= maxMushrooms)
            {
                DestroyRandomMushroom();
            }
        }
    }

    private void DestroyRandomMushroom()
    {
        if (mushrooms.Count > 0)
        {
            int randomIndex = Random.Range(0, mushrooms.Count); // Get a random index
            GameObject mushroomToDestroy = mushrooms[randomIndex];
            mushrooms.RemoveAt(randomIndex); // Remove from the list

            if (mushroomToDestroy != null)
            {
                 Destroy(mushroomToDestroy); // Destroy the mushroom GameObject
            }
        }
    }
    
}