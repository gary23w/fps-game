using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShareLocationNetwork : NetworkBehaviour
{
     [SyncVar] public Vector3 currentPosition = Vector3.zero;
     [SyncVar] public Quaternion currentRotation = Quaternion.identity;
 
     [ClientRpc]
     private void RpcSyncPositionWithClients(Vector3 positionToSync)
     {
         currentPosition = positionToSync;
     }
 
     [ClientRpc]
     private void RpcSyncRotationWithClients(Quaternion rotationToSync)
     {
         currentRotation = rotationToSync;
     }
 
     private void Update ()
     {
         if (isServer)
         {
             RpcSyncPositionWithClients(this.transform.position);
             RpcSyncRotationWithClients(this.transform.rotation);
             
         }
     }
 
     private void LateUpdate()
     {
         if (!isServer)
         {
             
             this.transform.position = currentPosition;
             this.transform.rotation = currentRotation;
            
         }
     }
 }
