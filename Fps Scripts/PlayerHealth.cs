using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerHealth : NetworkBehaviour, IEntity
{

    [SyncVar]
    public float health = 100f;
    public FloatVariable HealthFloatVariable;
    public FPScontroller player;
    void Awake() {
        
    }
    public void Update() {
        HealthFloatVariable.value = health;
    }



    public void Cmd_ApplyDamage(float damage) {
        if(!isServer) {
            return;
        }
        health -= damage;
        if(health <= 0f) {
            Debug.Log("DEAD MEAT");
            player.TeleportPlayer();
            health = 100f;

        }
    }
}
