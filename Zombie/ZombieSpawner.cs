using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ZombieSpawner : NetworkBehaviour
{
    
    public GameObject[] enemyPrefab;  
    public PlayerHealth player;
    // public Texture crosshairTexture;
    
    public float spawnInterval = 2; //Spawn new enemy each n seconds
    
    [SyncVar]
    
    public int enemiesPerWave = 5; //How many enemies per wave
    public Transform[] spawnPoints;
    
    float nextSpawnTime = 0;
    
    [SyncVar]
    
    int waveNumber = 1;

    [SyncVar]
    
    bool waitingForWave = true;
    
    [SyncVar]
    
    float newWaveTimer = 0;
    
    [SyncVar]
    int enemiesToEliminate;
    
    [SyncVar]
    
    int enemiesEliminated = 0;
    
    [SyncVar]   
    int totalEnemiesSpawned = 0;
    
    void Start()
    {
        //Lock cursor
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        newWaveTimer = 10;
        waitingForWave = true;
    }
    void Update()
    {
        CmdspawnZombies();
    }

    void OnGUI()
    {
        // GUI.Box(new Rect(10, Screen.height - 35, 100, 25), ((int)player.health).ToString() + " HP");
        GUI.Box(new Rect(Screen.width / 2 - 35, Screen.height - 35, 70, 25), newWaveTimer.ToString());

        // if(player.health <= 0)
        // {
        //     GUI.Box(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 20, 150, 40), "Game Over\nPress 'Space' to Restart");
        // }
        // else
        // {
        //     Debug.Log("TEXTURE STUFF CROSSHAIR");
        // }
        GUI.Box(new Rect(Screen.width / 2 - 50, 10, 100, 25), (enemiesToEliminate - enemiesEliminated).ToString());
        // if (waitingForWave)
        // {
        //     GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 4 - 12, 250, 25), "Waiting for Wave " + waveNumber.ToString() + ". " + ((int)newWaveTimer).ToString() + " seconds left...");
        // }
    }

    public void Cmd_EnemyEliminated()
    {
        enemiesEliminated++;

        if(enemiesToEliminate - enemiesEliminated <= 0)
        {
            //Start next wave
            newWaveTimer = 10;
            waitingForWave = true;
            waveNumber++;
        }
    }
    void CmdspawnZombies() {
    
    
    if (waitingForWave)
        {
            if(newWaveTimer >= 0)
            {
                newWaveTimer -= Time.deltaTime;
            }
            else
            {
                //Initialize new wave
                enemiesToEliminate = waveNumber * enemiesPerWave;
                enemiesEliminated = 0;
                totalEnemiesSpawned = 0;
                waitingForWave = false;
            }
        }
        else
        {
            if(Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + spawnInterval;

                //Spawn enemy 
                if(totalEnemiesSpawned < enemiesToEliminate)
                {
                    if(isServer) {
                    Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                    GameObject enemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)], randomPoint.position, Quaternion.identity);
                    NetworkServer.Spawn(enemy, connectionToClient);
                    ZombieController npc = enemy.GetComponent<ZombieController>();
                    npc.playerTransform = player.transform;
                    npc.es = this;
                    totalEnemiesSpawned++;
                    }
                    
                }
            }
        
        }

    }
}

