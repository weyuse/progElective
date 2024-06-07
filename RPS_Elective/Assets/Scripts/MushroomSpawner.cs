using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawner : MonoBehaviour
{
    public GameObject mushroomPrefab;

    // Interval between spawns in seconds
    public float spawnInterval = 20f; 

    public float arenaSize = 500f; 
    private float timer = 20f;

    void Start()
    {

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
            Vector3 spawnPosition = new Vector3(Random.Range(-arenaSize, arenaSize), 2, Random.Range(-arenaSize, arenaSize)
            );

            // instantiate the mushroom at the random position - quaternion meaning keeping the rotation as the prefab dictates
            Instantiate(mushroomPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
