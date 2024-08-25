using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PirateShipController : MonoBehaviour
{
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

    //casting Components
    private float castingCooldown = 1f;
    private bool canCast = true;
    public Transform ProjectileFrontSpawnPoint = null;
    public GameObject magicSpellPrefab = null;


    // Start is called before the first frame update
    void Start()
    {
        CalculateNavMeshBounds();
        currentMagicType = magicTypes[Random.Range(0, magicTypes.Length)];
        Debug.Log(currentMagicType);
        wizardMover = this.GetComponent<NavMeshAgent>();
    }

    // Assigns the AI that steers this instance
    public void SetAI(BaseAI _ai) 
    {
        ai = _ai;
        ai.Ship = this;
    }
        
    // Tell this ship to start battling
    public void StartBattle() 
    {
        StartCoroutine(ai.RunAI());
    }

    void Update()
    {
      
    }
    
    private void SetDestination(Vector3 destination)
    {
        if (wizardMover != null)
        {
            wizardMover.SetDestination(destination);
        }
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


    }

    public void hit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("wizard dead");
            
            killWiz();
        }
    }

    private void killWiz()
    {
        Destroy(gameObject, 1.0f);
    }

    public string GetCurrentMagicType()
    {
        return currentMagicType;
    }

    public int GetHealth()
    {
        return health;
    }

    public IEnumerator __DoNothing() {
        yield return new WaitForFixedUpdate();
    }

    //Moving after firing but keeping a certain distance depending on health

    public IEnumerator __SeekNewPosition(Transform target, float seekRadius, float spaceGiven)
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null. Aborting seek new position.");
            yield break; 
        }

        int maxAttempts = 10; 
        bool foundValidPoint = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * seekRadius;
            randomDirection += target.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, seekRadius, NavMesh.AllAreas))
            {
                wizardMover.SetDestination(hit.position);
                foundValidPoint = true;
                break; 
            }
        }

        if (!foundValidPoint)
        {
            Debug.LogWarning("Failed to find a valid position on the NavMesh within seekRadius after max attempts.");
            yield break;
        }

        while (wizardMover.pathPending || (wizardMover.remainingDistance > wizardMover.stoppingDistance))
        {
            if (target == null)
            {
                Debug.Log("Target is no longer visible, resetting path.");
                wizardMover.ResetPath();
                yield break;
            }

            if (target != null)
            {
                transform.LookAt(target);
            }

            yield return null; 
        }

        yield break; 
    }
   

    //Shooting as long as casting is off cooldown
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
            spellProjectile.SetColor(currentMagicType);
            spellProjectile.magicType = currentMagicType;
        }

        yield return new WaitForSeconds(castingCooldown);
        canCast = true;
        
    }

 
    // A random point a certain distance away from the target spotted
    public IEnumerator __Flee(Transform target)
    {
        float fleeDistance = 200f;
        int maxAttempts = 10; 
        bool foundValidPoint = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * fleeDistance;
            randomDirection += target.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, fleeDistance, NavMesh.AllAreas))
            {
                setDestination(hit.position);
                foundValidPoint = true;
                break;
            }
        }

        while (wizardMover.pathPending || wizardMover.remainingDistance > wizardMover.stoppingDistance)
        {
           yield return null;
        }

    }

    //Patrol checking if a target has been spotted where possible
    public IEnumerator __Patrol()
    {
        while (true)
        {
      
            if (ai.targetSpotted) 
            {
                wizardMover.ResetPath(); 
                yield break; 
            }

            Vector3 randomDestination = GetRandomPointOnNavMesh();
            setDestination(randomDestination);
            
            while (!ai.targetSpotted && (wizardMover.pathPending || wizardMover.remainingDistance > wizardMover.stoppingDistance))
            {
                if (ai.targetSpotted)
                {
                    wizardMover.ResetPath(); 
                    yield break; 
                }

                yield return null; 
            }

            if (!ai.targetSpotted)
            {
                yield return new WaitForSeconds(2f); 
            }
        }
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        Vector3 randomPoint = new Vector3(Random.Range(mapMinBounds.x, mapMaxBounds.x), 0f, Random.Range(mapMinBounds.z, mapMaxBounds.z));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 100f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    private void setDestination(Vector3 destination)
    {
        if (wizardMover != null)
        {
            wizardMover.SetDestination(destination);
        }
    }

    //Looks at the target and shoots them
    public IEnumerator __Engage(Transform target)
    {
        if (target == null) 
        {
            yield break; 
        }
        
        transform.LookAt(target.position);
        yield return StartCoroutine(__FireFront(1)); 
        yield break;        
    }

    //Randomly Selects a mushroom in the scene and moves towards it as long as the timer isnt 0
    public IEnumerator __GetMushroom()
    {
        GameObject[] mushroomPoint = GameObject.FindGameObjectsWithTag("magicMushroom");
        if (mushroomPoint.Length > 0)
        {
            GameObject randomMushroomPoint = mushroomPoint[Random.Range(0, mushroomPoint.Length)];
            setDestination(randomMushroomPoint.transform.position);
            float maxTimeToReach = 15f; 
            float startTime = Time.time;

            while (wizardMover.pathPending || wizardMover.remainingDistance > wizardMover.stoppingDistance)
            {
                if (Time.time - startTime > maxTimeToReach)
                {
                    Debug.Log("Giving up on reaching the mushroom, too far or unreachable.");
                    yield break; 
                }

                if (randomMushroomPoint == null)
                {
                    Debug.LogWarning("Mushroom was destroyed or is no longer available. Searching for another one.");
                    yield break;
                }

                yield return null;
            }
            Debug.Log("Reached the mushroom.");

            if (randomMushroomPoint != null)
            {
                string[] magicTypes = { "Fire", "Water", "Leaf" };
                string currentMagicType = magicTypes[Random.Range(0, magicTypes.Length)];
                this.GetComponent<PirateShipController>().currentMagicType = currentMagicType;
                Debug.Log("Mushroom picked");
                Destroy(randomMushroomPoint);
            }
        }

        else
        {
            Debug.LogWarning("No mushrooms found in the scene.");
            yield return null;
        }

        yield return new WaitForFixedUpdate();
    }
}
