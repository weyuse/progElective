﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// The event data that is generated when another participant in the arena was 'seen'
public class ScannedRobotEvent {
    public string Name;
    public float Distance;
    //also its position
    public Vector3 Position; 
    //also its magic type
    public string MagicType; 
    }

    //it complained unless I added monobehaviour
    public class BaseAI
{
    public PirateShipController Ship = null;

    // Another participant was 'seen'. Do something with the info stored in the even data
   
    public virtual void OnScannedRobot(ScannedRobotEvent e)
    {
      
    }

    public IEnumerator FireFront(float power)
    {
        yield return Ship.__FireFront(power);
    }

    // the flee command

    public IEnumerator Flee()
    {
        yield return Ship.__Flee();
    }

    public IEnumerator Engage(float angle)
    {
        yield return Ship.__Engage(angle);
    }

    public IEnumerator GetMushroom()
    {
        yield return Ship.__GetMushroom();
    }

    public IEnumerator Patrol()
    {
        yield return Ship.__Patrol();
    }


    public virtual IEnumerator RunAI() {
        yield return null;
    }
}
