using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

// The vehicle that will do battle. This is the same for every participant in the arena.
// Its 'brains' (the AI you'll write) will be assigned by the <seealso cref="CompetitionManager"/>

public class PirateShipController : MonoBehaviour
{
    // spell spawn area
    public Transform ProjectileFrontSpawnPoint = null;
    public GameObject magicSpellPrefab = null;

    // the 'scanner' that allows the ship to 'see' its surroundings
    public GameObject Lookout = null;

    //the AI that will control this ship. Is set by <seealso cref="CompetitionManager"/>.
    private BaseAI ai = null;
        
    // variables for the map size calculation so the random spots arent out of bounds
    public Vector3 mapMinBounds;
    public Vector3 mapMaxBounds;


    //health 
    private int health = 100;

    //magic types
    private string[] magicTypes = { "Fire", "Water", "Leaf" };

    //current magic type
    public string currentMagicType;

    //movement components
    public Transform targetDestination;
    public NavMeshAgent wizardMover;
    public Vector3 otherPosition;
    private float RotationSpeed;

    //cooldown for shooting to avoid machine-gun fire
    private float castingCooldown = 2f;
    private bool canCast = true;


    // Start is called before the first frame update
    void Start()
    {
        CalculateNavMeshBounds();

        //random magic type assigned at the start
        currentMagicType = magicTypes[Random.Range(0, magicTypes.Length)];
        Debug.Log(currentMagicType);

        //sets the nav agent to this game object
        wizardMover = this.GetComponent<NavMeshAgent>();
        RotationSpeed = wizardMover.angularSpeed;
    }

    // Assigns the AI that steers this instance

    public void SetAI(BaseAI _ai) {
        ai = _ai;
        ai.Ship = this;
    }

    // Tell this ship to start battling

    public void StartBattle() {
        StartCoroutine(ai.RunAI());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    // Calculate the bounds of the NavMesh through each vertex 
    private void CalculateNavMeshBounds()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        if (navMeshData.vertices.Length == 0)
        {
            Debug.LogError("where the navmesh");
            return;
        }

        mapMinBounds = navMeshData.vertices[0];
        mapMaxBounds = navMeshData.vertices[0];
             
        foreach (Vector3 vertex in navMeshData.vertices)
        {
            mapMinBounds = Vector3.Min(mapMinBounds, vertex);
            mapMaxBounds = Vector3.Max(mapMaxBounds, vertex);
        }

        Debug.Log("navMesh Bounds Min: " + mapMinBounds + ", Max: " + mapMaxBounds);
    }



    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Boat")
        {
            
            ScannedRobotEvent scannedRobotEvent = new ScannedRobotEvent();
            scannedRobotEvent.Distance = Vector3.Distance(transform.position, other.transform.position);
            scannedRobotEvent.Name = other.name;
      
            // Find the other's position and magic type
            scannedRobotEvent.Position = other.transform.position;

            // Look at the other ship's brain and tell me what's in it
            PirateShipController otherShip = other.transform.root.GetComponent<PirateShipController>();

            // Specifically their magic type
            if (otherShip != null) 
            {
                scannedRobotEvent.MagicType = otherShip.currentMagicType;
                if (ai == null)
                    return;

                ai.OnScannedRobot(scannedRobotEvent);
            }
            else
            {
                Debug.LogWarning("PirateShipController component not found on the root GameObject.");
            }
        }
    }


    //colliding with something tagged mushroom
    private void OnTriggerEnter(Collider other)
    {
        
         if (other.CompareTag("magicMushroom"))
        {
            currentMagicType = magicTypes[Random.Range(0, magicTypes.Length)];
            Destroy(other.gameObject);
        }
        
    }


    public void hit(int damage)
    {
        health -= damage;
        Debug.Log("Wizard Hit Damage:" + damage + "Remaining health:" + health);
        if (health <= 0)
        {
            Debug.Log("wizard dead");
            
            killWiz();
        }
    }

    private void killWiz()
    {
        Destroy(gameObject, 2.0f);
    }

    //telling the spell projectile script whats going on over here
    public string GetCurrentMagicType()
    {
        return currentMagicType;
    }

    public int GetHealth()
    {
        return health;
    }


    // Sit and hold still for one (fixed!) update

    public IEnumerator __DoNothing() {
        yield return new WaitForFixedUpdate();
    }

    // Fire front

    public IEnumerator __FireFront(float power)
    {
        if (!canCast)
        {
            yield break; 
        }

        canCast = false;

        GameObject newInstance = Instantiate(magicSpellPrefab, ProjectileFrontSpawnPoint.position, ProjectileFrontSpawnPoint.rotation);
        yield return new WaitForFixedUpdate();

        SpellProjectile spellProjectile = newInstance.GetComponent<SpellProjectile>();

        if (spellProjectile != null)
        {
            //sets the colour
            spellProjectile.SetColor(currentMagicType);
        }

        yield return new WaitForSeconds(castingCooldown);

        canCast = true;
    }

    // Flee method to move the ship away from the enemy

    public IEnumerator __Flee()
    {
        // find the corners and put them into an array
        GameObject[] respawnPoint = GameObject.FindGameObjectsWithTag("Respawn");
        if (respawnPoint.Length > 0)
        {
            GameObject randomRespawnPoint = respawnPoint[Random.Range(0, respawnPoint.Length)];
            // Set the target destination to a random point in the array
            setDestination(randomRespawnPoint.transform.position);
        }

        yield return new WaitForFixedUpdate(); 
    }


    public IEnumerator __Patrol()
    {
        Debug.Log("We do be patrolling");
        // Get a random point on the NavMesh
        Vector3 randomDestination = GetRandomPointOnNavMesh();
        setDestination(randomDestination);

        // Wait until dude reaches the destination
        while (!wizardMover.pathPending && wizardMover.remainingDistance > wizardMover.stoppingDistance)
        {
            yield return null; 
        }

        // Wait for a short duration before choosing a new destination
        yield return new WaitForSeconds(2f);
        
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        // Generate a random point within the map bounds
        Vector3 randomPoint = new Vector3(
            Random.Range(mapMinBounds.x, mapMaxBounds.x), 0f, Random.Range(mapMinBounds.z, mapMaxBounds.z)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 100f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid point is found, return the current position
        return transform.position;
    }

    //moving the wizard so long as it has a target to move to 
    private void setDestination(Vector3 destination)
    {
        if (wizardMover != null)
        {
            wizardMover.SetDestination(destination);
        }
    }

    public IEnumerator __Engage(Transform target)
    {
        /* wizardMover.isStopped = true;
         Debug.Log("Engaging the target");



              Debug.Log($"Aiming at: {target.name}");
             // Enable rotation towards the target
             wizardMover.updateRotation = true;

             // Set the agent to rotate towards the target's position
             wizardMover.SetDestination(target.position);

             // As we're using NavMeshAgent for rotation, but stopping it from moving, we reset its destination each frame
             wizardMover.isStopped = true;
             yield return new WaitForFixedUpdate(); // Wait for the next fixed frame update
        */
        Debug.Log("Engaging the target");

        while (true)
        {
            if (target == null)
            {
                Debug.Log("Target is null, stopping engagement.");
                yield break; // Stop the coroutine if the target is null
            }

            // Log target aiming
            Debug.Log($"Aiming at: {target.name}");

            // Enable rotation towards the target
            wizardMover.updateRotation = true;

            // Set the agent to rotate towards the target's position
            wizardMover.SetDestination(target.position);

            yield return StartCoroutine(__FireFront(1));

            // Wait for the next frame
            yield return null;

        }
    }

    public IEnumerator __GetMushroom()
    {
        // find the mushrooms and put them into an array
        GameObject[] mushroomPoint = GameObject.FindGameObjectsWithTag("magicMushroom");
        if (mushroomPoint.Length > 0)
        {
            GameObject randomMushroomPoint = mushroomPoint[Random.Range(0, mushroomPoint.Length)];
            // Set the target destination to a random point in the array
            setDestination(randomMushroomPoint.transform.position);
        }

        yield return new WaitForFixedUpdate();
    }
}
