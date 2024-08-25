using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PondAI : BaseAI
{

    private float healthThresholdLow = 50;
    private float healthThresholdMedium = 100;
    private string action;
    private Transform targetTransform;
    public float seekRadius;
    public float detectionAngle = 30f;
    public float detectionRange = 800f;
    public float spaceGiven;
    

    private void Start()
    {
       
    }

    private void Update()
    {
        PerformVisionConeDetection();
    }

    public void PerformVisionConeDetection()
    {
        targetSpotted = false;
        targetTransform = null;

        Collider ownCollider = GetComponent<Collider>();
        if (ownCollider == null)
        {
            Debug.LogWarning("Own collider is not found. Make sure this AI has a collider.");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (var collider in hitColliders)
        {
            if (collider == ownCollider) continue;

            if (collider.CompareTag("Boat"))
            {
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget < detectionAngle)
                {
                    Vector3 rayOrigin = transform.position;
                    Debug.DrawRay(rayOrigin, directionToTarget * detectionRange, Color.red, 0.1f);

                    RaycastHit hit;
                    if (Physics.Raycast(rayOrigin, directionToTarget, out hit, detectionRange))
                    {
                        Debug.Log("Raycast hit: " + hit.collider.name);

                        if (hit.collider.CompareTag("background"))
                        {
                            continue;
                        }

                        if (hit.collider == collider)
                        {
                            targetSpotted = true;
                            targetTransform = collider.transform;
                            PirateShipController shipController = hit.collider.GetComponentInParent<PirateShipController>();
                            string magicType = "Unknown";
                            if (shipController != null)
                            {
                                
                                magicType = shipController.GetCurrentMagicType();
                            }
                            ScannedRobotEvent scannedEvent = new ScannedRobotEvent
                            {
                                Name = collider.name,
                                Position = collider.transform.position,
                                MagicType = magicType
                            };
                            OnScannedRobot(scannedEvent);
                            return;
                        }
                    }
                }
            }
        }
        targetSpotted = false;
    }
    // Visualize the vision cone using Gizmos
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Only draw when the game is running

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw vision cone
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle, 0) * transform.forward * detectionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    public override IEnumerator RunAI()
    {
        while (true)
        {
            Debug.Log("Current Action:" + action);

            if (targetTransform == null)
            {
                yield return Patrol();
            }
            
                                   
            switch (action)
            {
                case "engage":                    
                    yield return Engage(targetTransform);
                    yield return SeekNewPosition(targetTransform, seekRadius, spaceGiven);
                    break;

                case "flee":
                    yield return Flee(targetTransform);
                    targetTransform = null;
                    break;

                case "findMushroom":
                    targetTransform = null;
                    yield return GetMushroom();
                    break;

                case "patrol":
                    yield return Patrol();
                    break;

                default:
                    action = "patrol";
                    break;
            }
            yield return new WaitForSeconds(1f);
        }
       
    }

    private void ResetTargetInformation()
    {
        Debug.Log("Resetting target information... SIKE");
        targetTransform = null; // Clear the target reference
    }


    public override void OnScannedRobot(ScannedRobotEvent e)
    {
       
       // GameObject targetObject = GameObject.Find(e.Name);
        //Transform wizardBody = targetObject.transform.Find("WizardBody");
        //targetTransform = wizardBody;
        float health = Ship.GetHealth();
        string myMagicType = Ship.GetCurrentMagicType();
        string enemyMagicType = e.MagicType;
        action = DetermineAction(health, myMagicType, enemyMagicType);
                
    }

    private bool IsAdvantageousMagic(string myMagicType, string enemyMagicType)
    {
        Debug.Log(enemyMagicType);
        if ((myMagicType == "Fire" && enemyMagicType == "Leaf") ||
            (myMagicType == "Leaf" && enemyMagicType == "Water") ||
            (myMagicType == "Water" && enemyMagicType == "Fire"))
        {
            return true;
        }
        return false;
    }

    private bool IsSameMagic(string myMagicType, string enemyMagicType)
    {
        Debug.Log(enemyMagicType);
        if ((myMagicType == "Fire" && enemyMagicType == "Fire") ||
            (myMagicType == "Leaf" && enemyMagicType == "Leaf") ||
            (myMagicType == "Water" && enemyMagicType == "Water"))
        {
            return true;
        }
        return false;
    }

    public string DetermineAction(float health, string myMagicType, string enemyMagicType)
    {
        // High health wizards always engage and get closer
        if (health > healthThresholdMedium)
        {
            seekRadius = 100f; // Smaller radius, gets closer
            spaceGiven = 50f;
            Debug.Log("High health detected, engaging with close seek radius.");
            return "engage";
        }

        // Medium health wizards engage if they have an advantage or aren't at a disadvantage
        if (health > healthThresholdLow && health <= healthThresholdMedium)
        {
            seekRadius = 150f;
            spaceGiven = 75f;// Medium radius
            if (IsAdvantageousMagic(myMagicType, enemyMagicType))
            {
                Debug.Log("Medium health with magic advantage, engaging with medium seek radius.");
                return "engage";
            }
            else if (IsSameMagic(myMagicType, enemyMagicType)) // Check if both have the same magic type
            {
                Debug.Log("Medium health, same magic type detected, engaging with medium seek radius.");
                return "engage";
            }
            else
            {
                Debug.Log("Medium health, at disadvantage, finding mushroom.");
                return "findMushroom";
            }
        }

        // Low health wizards engage only if they have an advantage, otherwise flee
        if (health <= healthThresholdLow)
        {
            seekRadius = 200f;
            spaceGiven = 100f;// Larger radius, keeps distance
            if (IsAdvantageousMagic(myMagicType, enemyMagicType))
            {
                Debug.Log("Low health with magic advantage, engaging with large seek radius.");
                return "engage";
            }
            else
            {
                Debug.Log("Low health, no advantage, fleeing.");
                return "flee";
            }
        }

        // Fallback to patrol action if none of the conditions match
        Debug.Log("Defaulting to patrol.");
        return "patrol";
    }
}
   
