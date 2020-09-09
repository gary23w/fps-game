using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    private bool firstPlayerJoined;
    public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerID) {
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        List<Transform> spawnPositions = NetworkManager.singleton.startPositions;

        if(!firstPlayerJoined) {
            firstPlayerJoined = true;
            playerObj.transform.position = spawnPositions [0].position;

        } else {
            playerObj.transform.position = spawnPositions [1].position;
        }
        NetworkServer.AddPlayerForConnection (connection, playerObj, playerControllerID);
    }
    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.networkAddress = "localhost";
    }
    public void HostGame() {
        SetPort();
        NetworkManager.singleton.StartHost();

    }

    public void Join() {
        SetPort();
        NetworkManager.singleton.StartClient();
    }
}
