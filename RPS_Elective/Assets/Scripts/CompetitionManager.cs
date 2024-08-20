using System.Collections.Generic;
using UnityEngine;


public class CompetitionManager : MonoBehaviour
{
    // the prefab for the participants in the battle
    public GameObject PirateShipPrefab = null;

    // The positions where the participants will be instantiated
    // set in the inspector by dragging 4 gameobjects in the slots of the array
    public Transform[] SpawnPoints = null;

    // the list that keeps track of all the participants
    private List<PirateShipController> pirateShips = new List<PirateShipController>();

   
    void Start()
    {
        BaseAI[] aiArray = new BaseAI[] {
            new PondAI(),
            new PondAI(),
            new PondAI(),
            new PondAI()
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject pirateShip = Instantiate(PirateShipPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
            PirateShipController pirateShipController = pirateShip.GetComponent<PirateShipController>();
            pirateShipController.SetAI(aiArray[i]);
            pirateShips.Add(pirateShipController);
        }

        foreach(var pirateShip in pirateShips) {
            pirateShip.StartBattle();
        }
    }

    void Update()
    {
               
    }
}
