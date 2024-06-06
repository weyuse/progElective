using System.Collections;




/// You can modify this template to make your own AI

public class PondAI : BaseAI
{
    public override IEnumerator RunAI() {
        while (true)
        {
            yield return Ahead(500);
            yield return FireFront(1);
            yield return TurnRight(180);
            
        }
    }

 
    public override void OnScannedRobot(ScannedRobotEvent e)
    {
    }
}
