using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PondAI : BaseAI
{

    private float healthThresholdLow = 800;
    private float healthThresholdMedium = 950;
    private string action = "patrol";
    private Transform targetTransform;


    private void Start()
    {
        //agent = GetComponent<NavMeshAgent>();

    }

    public override IEnumerator RunAI()
    {
        while (true)
        {
            Debug.Log("Current Action:" + action);
                        
            switch (action)
            {
                case "engage":
                    yield return Engage(targetTransform);
                    break;

                case "flee":
                    yield return Flee(targetTransform);
                    break;

                case "findMushroom":
                    yield return GetMushroom();
                    break;

                case "patrol":
                    yield return Patrol();
                    break;

                default:
                    action = "patrol";
                    break;
            }
           

            yield return null;  
        }
       
    }
    



    public override void OnScannedRobot(ScannedRobotEvent e)
    {
        GameObject targetObject = GameObject.Find(e.Name);

        if (targetObject != null && targetObject.CompareTag("Boat"))
        {
            Transform wizardBody = targetObject.transform.Find("WizardBody");
            if (wizardBody != null)
            {
                targetTransform = wizardBody;
                float health = Ship.GetHealth();
                string myMagicType = Ship.GetCurrentMagicType();
                string enemyMagicType = e.MagicType;
                action = DetermineAction(health, myMagicType, enemyMagicType);
            }
            else
            {
                Debug.Log("no WizardBody part found");
            }
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
   
