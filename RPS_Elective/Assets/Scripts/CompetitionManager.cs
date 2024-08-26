using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CompetitionManager : MonoBehaviour
{
    // the prefab for the participants in the battle
    public GameObject PirateShipPrefab = null;

    // The positions where the participants will be instantiated
    // set in the inspector by dragging 4 gameobjects in the slots of the array
    public Transform[] SpawnPoints = null;

    // the list that keeps track of all the participants
    private List<PirateShipController> pirateShips = new List<PirateShipController>();

    // button functionality
    public Button StartRestartButton;
    public Text ButtonText;
    private bool battleStarted = false;

    private audioControl audioController;


    void Start()
    {
        StartRestartButton.onClick.AddListener(OnStartRestartButtonClicked);
        ButtonText.text = "Start";

        GameObject audioManager = GameObject.Find("Audio Source");
        if (audioManager != null)
        {
            audioController = audioManager.GetComponent<audioControl>();
        }
    }
    public void OnStartRestartButtonClicked()
    {
        if (battleStarted)
        {
            RestartBattle();
        }
        else
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject pirateShip = Instantiate(PirateShipPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
            PirateShipController pirateShipController = pirateShip.GetComponent<PirateShipController>();
            PondAI pondAI = pirateShip.AddComponent<PondAI>();
            pirateShipController.SetAI(pondAI);
            pirateShips.Add(pirateShipController);
        }

        foreach (var pirateShip in pirateShips)
        {
            pirateShip.StartBattle();
        }

        audioController.PlaySound(audioController.backgroundMusic);

        battleStarted = true;
        ButtonText.text = "Restart";

    }

    private void RestartBattle()
    {
        foreach (var pirateShip in pirateShips)
        {
            Destroy(pirateShip.gameObject);
        }
        pirateShips.Clear();
        if (audioController != null)
        {
            audioController.GetComponent<AudioSource>().Stop();
        }
        battleStarted = false;
        StartBattle();
    }

    public bool IsBattleStarted()
    {
        return battleStarted;
    }

    void Update()
    {
               
    }
}
