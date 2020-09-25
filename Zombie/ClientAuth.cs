using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientAuth : NetworkBehaviour
{
    GameObject currentPLAYER;


    void Awake() {   
    }
    
    void Update()
     {
                          
    //                         var playerID = gameObject.GetComponent<NetworkIdentity>();     
    //                         CmdSetAuth(netId, playerID);        
        
    }

            // [Command]
            // public void CmdSetAuth(uint objectId, NetworkIdentity player)
            // {
            //     // var iObject = NetworkIdentity.spawned[objectId];
            //     var networkIdentity = gameObject.GetComponent<NetworkIdentity>();
            //     var otherOwner = networkIdentity.clientAuthorityOwner;        
        
            //     if (otherOwner == player.connectionToClient)
            //     {
            //         return;
            //     }
            //     else
            //     {
            //         if (otherOwner != null)
            //         {
            //             // networkIdentity.RemoveClientAuthority(otherOwner);
            //         }
            //         networkIdentity.AssignClientAuthority(player.connectionToClient);
            //     }        
            // }
}
