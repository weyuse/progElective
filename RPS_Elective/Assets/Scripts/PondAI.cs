using System.Collections;
using UnityEngine;



/// You can modify this template to make your own AI

public class PondAI : BaseAI
{

    private float healthThresholdLow = 30;
    private float healthThresholdMedium = 60;
    private string action;

    public override IEnumerator RunAI() {
        while (true)
        {
            if (action == "engage")
            {
                yield return Engage(2);
                yield return FireFront(1);

            }
            else if (action == "flee")
            {
                yield return Flee();
  
            }
            else if(action == "findMushroom")
            {
                yield return GetMushroom();
            }
            else
            {
                //generic patrol
                yield return TurnRight(90);
                yield return TurnLeft(180);
                yield return TurnRight(90);
                yield return Ahead(200);
            }
        } 
    }

    /*public override IEnumerator Flee()
    {
        while (true)
        {
            //flee
            yield return Flee(20);

        }
    }
    */


    public override void OnScannedRobot(ScannedRobotEvent e)
    {
        //what are the factors that influence my decision
        float health = Ship.GetHealth();
        string myMagicType = Ship.GetCurrentMagicType();
        string enemyMagicType = e.MagicType; 

        //what am I gonnna do
        string action = DetermineAction(health, myMagicType, enemyMagicType);
        
        if (action == "engage")
        {
           // Ship.StartCoroutine(Flee(300));
        }
        else if (action == "flee")
        {
            Debug.Log("Get outta there kid!");

            //Ship.StartCoroutine(Flee(300));
        }
        else if (action == "findMushroom")
        {
           // Ship.StartCoroutine(Flee(300));
        }
        
        Debug.Log(action);
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
        if (IsAdvantageousMagic (myMagicType, enemyMagicType))
        {
            return "engage";
        }

        else
        {
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
                if (IsAdvantageousMagic(myMagicType, enemyMagicType))
                {
                    return "engage";
                }
                //when all else fails flee
                else
                {
                    return "flee";
                }
            }
        }
    }

    
}
