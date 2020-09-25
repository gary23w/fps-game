using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Networking;
// using UnityEngine.Networking.Match;
using Mirror;

public class NetManager : NetworkManager
{
    // private bool firstPlayerJoined;
    // public GameObject GamePanel;
    // public GameObject ZombieSpawner;

    // public GameObject playerPrefab;

    
    // public override void OnServerAddPlayer(NetworkConnection conn) {
    //     base.OnServerAddPlayer(conn);
    //     GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    //     List<Transform> spawnPositions = NetworkManager.startPositions;

    //     if(!firstPlayerJoined) {
    //         firstPlayerJoined = true;
    //         playerObj.transform.position = spawnPositions [0].position;

    //     } else {
    //         playerObj.transform.position = spawnPositions [1].position;
    //     }
    //     NetworkServer.AddPlayerForConnection (conn, playerObj);
    // }
    // void SetPort() {
    //     // NetworkManager.networkPort = 7777;
    //     // NetworkManager.networkAddress = "localhost";
    // }
    // public void HostGame() {
    //     SetPort();
    //     NetworkManager.StartHost();
       

    // }

    // public void Join() {
    //     SetPort();
    //     NetworkManager.StartClient();
           
    // }
    // public void JoinOnlineMatch() {
    //     NetworkManager.StartMatchMaker();
         
        
    // }
    // public void CreateOnlineMatch() {
    //     NetworkManager.matchMaker.CreateMatch("roomName", 4, true, "", "", "", 0, 0, "OnMatchCreate");
           
    // }

    // // public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    // // {
    // //     Debug.Log("match created");
    // // }
    // public void offlineMode() {
    //     GamePanel.transform.localScale = Vector3.zero;
    //     playerPrefab.gameObject.SetActive(true);

    // }
}
