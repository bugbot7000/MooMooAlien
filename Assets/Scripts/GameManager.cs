using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CowSpawner cowMaker;

    public AlienSpawner alienMaker;

    public UnityEvent onGameStart;
    public bool GameIsOver;
    public GameObject controllerCanvas, scoreKeeperCanvas;
    public AudioSource BGM;
    public int shotsFired, shotsHit, shotsMissed;
    public CanvasController canCon;
    public ETFXFireProjectile gunL, gunR;

    [Header("Save Data")]
    public int playerNumber;
    public int kills;
    public int bulletsFired;
    public int saves;
    public TextMeshProUGUI scoreText;
    public int aliensKilled,cowsSaved;
    // Start is called before the first frame update

    private void Start()
    {
        LoadData();
    }
    [Button("Load Data")]
    private void LoadData()
    {
        if (SaveGame.Exists("kills"))
        {
            Debug.Log("Data Exists");
            playerNumber = SaveGame.Load<int>("playerCount");
            bulletsFired = SaveGame.Load<int>("shotsFired");
            kills = SaveGame.Load<int>("kills");
            saves = SaveGame.Load<int>("saves");
        }
        else
        {
            Debug.Log("Database Made");
            SaveValues(false);
        }
    }

    [Button("Start Game")]
    public void StartGame()
    {
        playerNumber++;
        gunL.gameObject.SetActive(true);
        gunR.gameObject.SetActive(true);
        gunL.canFire = true;
        gunR.canFire = true;
        controllerCanvas.gameObject.SetActive(false);
        onGameStart.Invoke();
    }

    private void Update()
    {
        shotsFired = gunL.ShotsFiredFromGun + gunR.ShotsFiredFromGun;
        shotsMissed = shotsFired - shotsHit;
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveValues(true);
        }

        if (BGM.isPlaying == false && shotsFired > 1)
        {
            canCon.GameOver = true;
            GameIsOver = true;
            SaveValues(true);
            ShowEndPanel();
            BGM.volume = 0;
            BGM.Play();
        }
    }
    public void SaveValues(bool setVals)
    {
        if (setVals)
        {
            playerNumber++;
            bulletsFired += shotsFired;
            kills += aliensKilled;
            saves += cowsSaved;
        }
        SaveGame.Save<int>("playerCount",playerNumber);
        SaveGame.Save<int>("shotsFired",bulletsFired);
        SaveGame.Save<int>("kills",kills);
        SaveGame.Save<int>("saves",saves);
        Debug.Log("Saved");
    }
    [Button("Clear Saves")]
    public void ClearSaves()
    {
        SaveGame.DeleteAll();
    }
    [Button("SetFinals")]
    public void ShowEndPanel()
    {
        if (GameIsOver)
        {
            canCon.ToggleScoreCanvas();
            float accuracy = (shotsHit / shotsFired) * 100;
            //float accuracy = 35.26f;
            string FinalText = "\nShots fired: " + shotsFired +
                               "\nShots hit: " + shotsHit + "\nShots missed: " + shotsMissed + "\n Accuracy: " +
                               accuracy + "%" + "\n Aliens Purged: " + aliensKilled + "\nCows Rescued: " + cowsSaved +
                               "\n\n Thank You For Playing \n\n Please Press Right Thumbstick to Exit";
            scoreText.text = FinalText;
        }
    }

    public void RestartGame()
    {
        if(GameIsOver)
        SceneManager.LoadScene(0);
    }
}
