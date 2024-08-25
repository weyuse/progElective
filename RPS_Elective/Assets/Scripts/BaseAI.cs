using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// The event data that is generated when another participant in the arena was 'seen'
public class ScannedRobotEvent 
{
    public string Name;
    public float Distance;
    public Vector3 Position; 
    public string MagicType; 
}

public class BaseAI : MonoBehaviour
{
    public PirateShipController Ship = null;
    public bool targetSpotted;
 
    public virtual void OnScannedRobot(ScannedRobotEvent e)
    {

    }

    public IEnumerator FireFront(float power)
    {
        yield return Ship.__FireFront(power);
    }

    public IEnumerator Flee(Transform target)
    {
        yield return Ship.__Flee(target);
    }

    public IEnumerator Engage(Transform target)
    {
        yield return Ship.__Engage(target);
    }

    public IEnumerator GetMushroom()
    {
        yield return Ship.__GetMushroom();
    }

    public IEnumerator Patrol()
    {
        yield return Ship.__Patrol();
    }
    public IEnumerator SeekNewPosition(Transform target, float seekRadius, float spaceGiven)
    {
        yield return Ship.__SeekNewPosition(target, seekRadius, spaceGiven);
    }

    public virtual IEnumerator RunAI() 
    {
        yield return null;
    }
}
