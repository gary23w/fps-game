using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkDestroyObjectClient : NetworkBehaviour
{
    // Called by the Player
    [Client]
    public void TellServerToDestroyObject(GameObject obj)
    {
        CmdDestroyObject(obj);
    }

    // Executed only on the server
    [Command]
    private void CmdDestroyObject(GameObject obj)
    {
        if(!obj) return;

        NetworkServer.Destroy(obj);
    }
}
