using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// The event data that is generated when another participant in the arena was 'seen'
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

    /// Another participant was 'seen'. Do something with the info stored in the even data
   
    public virtual void OnScannedRobot(ScannedRobotEvent e)
    {
      
    }

    /// Move this ship ahead by the given distance
  
    public IEnumerator Ahead(float distance) {
        yield return Ship.__Ahead(distance);
    }

    /// Move the ship backwards by the given distance
    /// 
    public IEnumerator Back(float distance) {
        yield return Ship.__Back(distance);
    }

    /// Turn the sensor to the left by the given angle

    public IEnumerator TurnLookoutLeft(float angle) {
        yield return Ship.__TurnLookoutLeft(angle);
    }

    /// Turn the sensor to the right by the given angle

    public IEnumerator TurnLookoutRight(float angle) {
        yield return Ship.__TurnLookoutRight(angle);
    }

    /// Turns the ship left by the given angle
    
    public IEnumerator TurnLeft(float angle) {
        yield return Ship.__TurnLeft(angle);
    }

    /// Turns the ship right by the given angle

    public IEnumerator TurnRight(float angle) {
        yield return Ship.__TurnRight(angle);
    }

    /// Fire from the forward pointing cannon

    public IEnumerator FireFront(float power) {
        yield return Ship.__FireFront(power);
    }

    public virtual IEnumerator RunAI() {
        yield return null;
    }
}
