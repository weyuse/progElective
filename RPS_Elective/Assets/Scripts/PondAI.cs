﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PondAI : BaseAI
{

    private float healthThresholdLow = 30;
    private float healthThresholdMedium = 60;
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
                    Debug.Log("Lets get engaging");
                    yield return Engage(targetTransform);
                    //yield return FireFront(1);
                    break;

                case "flee":
                    yield return Flee();
                    break;

                case "findMushroom":
                    yield return GetMushroom();
                    break;

                case "patrol":
                    // generic patrol
                    yield return Patrol();
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


    private string DetermineAction(float health, string myMagicType, string enemyMagicType)
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
   
