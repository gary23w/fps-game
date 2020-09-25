using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject playerObject;
    public GameObject ZombieSpawner;
    public GameObject map;
    public GameObject netManager;


    public void offlineMode() {
      
        mainMenuPanel.transform.localScale = Vector3.zero;
        playerObject.gameObject.SetActive(true);
        ZombieSpawner.SetActive(true);
        map.SetActive(true);

    }
}
