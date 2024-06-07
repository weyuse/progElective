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

    // create a level playing field. Every ship has the same basic abilities
    private float WizSpeed = 100.0f;
    private float ArenaSize = 500.0f;
    private float RotationSpeed = 180.0f;

    //health and damage for winning competition purposes
    private int health = 100;
    private int damage = 10;

    //magic types
    private string[] magicTypes = { "Fire", "Water", "Leaf" };

    //current magic type
    private string currentMagicType;

    public Transform targetDestination;

    public NavMeshAgent wizardMover;

    public Vector3 otherPosition;

    //animator for polish
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        //random magic type assigned at the start
        currentMagicType = magicTypes[Random.Range(0, magicTypes.Length)];

        //sets the nav agent to this game object
        wizardMover = this.GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
       
    }

 
        
       

    // Assigns the AI that steers this instance

    public void SetAI(BaseAI _ai) {
        ai = _ai;
        ai.Ship = this;
    }

    // Tell this ship to start battling

    public void StartBattle() {
        Debug.Log("test");
        StartCoroutine(ai.RunAI());
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }




    // If a ship is inside the 'scanner', its information (distance and name) will be sent to the AI

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Boat")
        {
            Debug.Log("I see somethin");
            ScannedRobotEvent scannedRobotEvent = new ScannedRobotEvent();
            scannedRobotEvent.Distance = Vector3.Distance(transform.position, other.transform.position);
            scannedRobotEvent.Name = other.name;
            Debug.Log(other.name);
            //find the others position and magic type
            scannedRobotEvent.Position = other.transform.position;
            //look at the other ships brain and tell me whats in it
            PirateShipController otherShip = other.GetComponent<PirateShipController>();
            //specifcally their magic type
            scannedRobotEvent.MagicType = otherShip.currentMagicType;
            Debug.Log(ai);
            ai.OnScannedRobot(scannedRobotEvent);
            //this event is in BaseAI btw
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


    // Move ahead by the given distance

    public IEnumerator __Ahead(float distance) {
        int numFrames = (int)(distance / (WizSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Translate(new Vector3(0f, 0f, WizSpeed * Time.fixedDeltaTime), Space.Self);
            Vector3 clampedPosition = Vector3.Max(Vector3.Min(transform.position, new Vector3(ArenaSize, 0, ArenaSize)), new Vector3(-ArenaSize, 0, -ArenaSize));
            transform.position = clampedPosition;

            yield return new WaitForFixedUpdate();
        }
    }

    // Move backwards by the given distance

    public IEnumerator __Back(float distance) {
        int numFrames = (int)(distance / (WizSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Translate(new Vector3(0f, 0f, -WizSpeed * Time.fixedDeltaTime), Space.Self);
            Vector3 clampedPosition = Vector3.Max(Vector3.Min(transform.position, new Vector3(ArenaSize, 0, ArenaSize)), new Vector3(-ArenaSize, 0, -ArenaSize));
            transform.position = clampedPosition;

            yield return new WaitForFixedUpdate();
        }
    }

    // Turns left by the given angle

    public IEnumerator __TurnLeft(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Rotate(0f, -RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();
        }
    }

    // Turns right by the given angle

    public IEnumerator __TurnRight(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Rotate(0f, RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();
        }
    }

    // Sit and hold still for one (fixed!) update

    public IEnumerator __DoNothing() {
        yield return new WaitForFixedUpdate();
    }

    // Fire front

    public IEnumerator __FireFront(float power)
    {
        animator.Play("Attack01");
        
        GameObject newInstance = Instantiate(magicSpellPrefab, ProjectileFrontSpawnPoint.position, ProjectileFrontSpawnPoint.rotation);
        yield return new WaitForFixedUpdate();

        SpellProjectile spellProjectile = newInstance.GetComponent<SpellProjectile>();

        if (spellProjectile != null)
        {
            //sets the colour
            spellProjectile.SetColor(currentMagicType);
        }
    }


    // Turn the sensor to the left by the given angle

    public IEnumerator __TurnLookoutLeft(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            Lookout.transform.Rotate(0f, -RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();
        }
    }

    // Turn the sensor to the right by the given angle

    public IEnumerator __TurnLookoutRight(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            Lookout.transform.Rotate(0f, RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();
        }
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

    //moving the wizard so long as it has a target to move to 
    private void setDestination(Vector3 destination)
    {
        if (wizardMover != null)
        {
            wizardMover.SetDestination(destination);
        }
    }

    public IEnumerator __Engage(float angle)
    {
       
            int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
            for (int f = 0; f < numFrames; f++)
            {
                Lookout.transform.Rotate(0f, -RotationSpeed * Time.fixedDeltaTime, 0f);

                yield return new WaitForFixedUpdate();
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
