using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PondAI : BaseAI
{

    private float healthThresholdLow = 100;
    private float healthThresholdMedium = 200;
    private string action;
    private Transform targetTransform;
    public float seekRadius;


    private void Start()
    {
        //agent = GetComponent<NavMeshAgent>();

    }

    public override IEnumerator RunAI()
    {
        while (true)
        {
            if (Ship != null)
            {
                Ship.PerformVisionConeDetection();
            }

            Debug.Log("Current Action:" + action);
                        
            switch (action)
            {
                case "engage":
                    //if (targetTransform != null)
                    //{
                    //    yield return Engage(targetTransform);
                    //}
                    //ResetTargetInformation();
                    break;

                case "flee":
                    yield return Flee(targetTransform);
                    // Reset target information after each action cycle
                    //ResetTargetInformation();
                    yield return Patrol();
                    break;

                case "findMushroom":
                    yield return GetMushroom();
                    // Reset target information after each action cycle
                    //ResetTargetInformation();
                    yield return Patrol();
                    break;

                case "patrol":
                    yield return Patrol();
                    break;

                default:
                    action = "patrol";
                    break;
            }


            yield return new WaitForSeconds(0.5f);
        }
       
    }

    private void ResetTargetInformation()
    {
        Debug.Log("Resetting target information... SIKE");
        //targetTransform = null; // Clear the target reference
    }


    public override void OnScannedRobot(ScannedRobotEvent e)
    {
        GameObject targetObject = GameObject.Find(e.Name);
        Debug.Log($"OnScannedRobot called with target: {e.Name}, Distance: {e.Distance}, MagicType: {e.MagicType}");
        if (targetObject != null && targetObject.CompareTag("Boat"))
        {
            Transform wizardBody = targetObject.transform.Find("WizardBody");
            if (wizardBody != null)
            {
                targetTransform = wizardBody;
                float health = Ship.GetHealth();
                string myMagicType = Ship.GetCurrentMagicType();
                string enemyMagicType = e.MagicType;
                Debug.Log("About to determine action");
                action = DetermineAction(health, myMagicType, enemyMagicType);
            }
            else
            {
                Debug.Log("no WizardBody part found");
                ResetTargetInformation();
            }
        }
        else
        {
            ResetTargetInformation();
        }
    }

    private bool IsAdvantageousMagic(string myMagicType, string enemyMagicType)
    {
        if ((myMagicType == "Fire" && enemyMagicType == "Leaf") ||
            (myMagicType == "Leaf" && enemyMagicType == "Water") ||
            (myMagicType == "Water" && enemyMagicType == "Fire"))
        {
            return true;
        }
        return false;
    }


    public string DetermineAction(float health, string myMagicType, string enemyMagicType)
    {

        // higher health wizards get closer
        if (health <= healthThresholdLow)
        {
            seekRadius = 500f; 
        }
        else if (health > healthThresholdLow && health <= healthThresholdMedium)
        {
            seekRadius = 1000f; 
        }
        else
        {
            seekRadius = 1500f; 
        }

        //magic advantage makes them confident
        if (IsAdvantageousMagic(myMagicType, enemyMagicType))
        {
                return "engage";
        }
            
            //low health wizard is always a coward
            if (health <= healthThresholdLow)
            {
                return "flee";
            }
            //medium health wizard will try and find an advantage
            else if (health > healthThresholdLow && health <= healthThresholdMedium)
            {
                return "findMushroom";
            }
            //high health wizard will try their luck
            else
            {
                return "engage";
            }
       
    }
}
   
