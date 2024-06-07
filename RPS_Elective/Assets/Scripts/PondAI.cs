using System.Collections;
using UnityEngine;



/// You can modify this template to make your own AI

public class PondAI : BaseAI
{

    private float healthThresholdLow = 30;
    private float healthThresholdMedium = 60;

    public override IEnumerator RunAI() {
        while (true)
        {
            //generic look left and right for enemies before combat starts
            yield return TurnRight(90);
            yield return TurnLeft(180);
            yield return TurnRight(90);
            yield return Ahead(200);
            
        }
    } 

 
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
            EngageEnemy(e);
        }
        else if (action == "flee")
        {
            FleeEnemy(e);
        }
        else if (action == "findMushroom")
        {
            FindMushroom(e);
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
                
        private void EngageEnemy(ScannedRobotEvent e)
        {
            Debug.Log("We Shootin fr");
            //temp
        }

       private void FleeEnemy(ScannedRobotEvent e)
        {
            Debug.Log("Get outta there kid!");
            
        }

        private void FindMushroom(ScannedRobotEvent e)
        {
            Debug.Log("Gotta switch it up");
            
        }
    }
