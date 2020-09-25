using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BodyguardController :  NetworkBehaviour, IEntity
{
    public Animator zombieAnimations;
    private NetworkAnimator networkAnim;
    public string IDLE = "Idle";
    public string WALK = "Walking";

    public string RUN = "Running";
    public string ATTACK = "Attacking";
    public string DEAD = "Dead";
    public string WALKSLOW = "WalkingSlow";

    public Transform[] points;
    private int destPoint = 0;
    public NavMeshAgent agent;
    public float attackDistance = 3f;
    public float movementSpeed = 10f;
    [SyncVar]
    public float npcHP = 100;
    //How much damage will npc deal to the player
    public float npcDamage = 25;
    public float attackRate = 0.5f;
    public Transform firePoint;
    Transform playerTransform;
    float nextAttackTime = 0;
    Rigidbody r;
    PlayerHealth PLAYER_MAIN;
    Transform playerTag;
    NetworkIdentity networkIdentity;
    public GameObject blood_Impact;
    void Awake()
    {
        zombieAnimations = GetComponent<Animator>();
        networkAnim = GetComponent<NetworkAnimator>();
        agent = GetComponent<NavMeshAgent>(); 
        networkIdentity = GetComponent<NetworkIdentity>();
        playerTransform = GameObject.FindWithTag("Zombie").transform;

    }

    void Start () {
            agent.autoBraking = false;
            agent.stoppingDistance = attackDistance;
            agent.speed = movementSpeed;
            r = GetComponent<Rigidbody>();
            r.useGravity = false;
            r.isKinematic = true; 
        }
        void Update () {
                    try {
                        playerTag = GameObject.FindWithTag("Player").transform;
                    } catch {
                        playerTag = playerTransform;
                    }
                    agent.destination = playerTag.transform.position;
                    transform.LookAt(new Vector3(playerTag.transform.position.x, transform.position.y, playerTag.position.z));
                    r.velocity *= 0.99f;          
                    WALK_Z(1.0f);
                    
                        
        
    if (agent.remainingDistance - attackDistance < 0.01f)
        {
            if(Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                //Attack
                RaycastHit hit;
                if(Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackDistance))
                {
                    if (hit.transform.tag == "Zombie")
                    {
                         ATTACK_Z();
                         //  Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * attackDistance, Color.cyan);
                         agent.destination = playerTransform.position;
                         IEntity player = hit.transform.GetComponent<IEntity>();
                         Instantiate(blood_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                         player.Cmd_ApplyDamage(npcDamage);
                         Debug.Log("BODDYGUARD ATTACKING ZOMBIE");     
                         
                    }
                }
            }
        }          
     
}
                    public void WALK_Z(float isWalking_Z) {
                        zombieAnimations.SetFloat(WALK, isWalking_Z);
                    }
                    public void ATTACK_Z() {
                        zombieAnimations.SetTrigger(ATTACK);
                        networkAnim.SetTrigger(ATTACK);
                    }
                    public void IDLE_Z() {
                        zombieAnimations.SetTrigger (IDLE);
                        networkAnim.SetTrigger(IDLE);
                    }
                    public void DEAD_Z() {
                        zombieAnimations.SetTrigger (DEAD);
                        networkAnim.SetTrigger(DEAD);
                    }
                    public void WALKSLOW_Z() {
                        zombieAnimations.SetTrigger(WALKSLOW);
                    }


            
            public void Cmd_ApplyDamage(float points)
                { 
                    npcHP -= points;
                    if(npcHP <= 0)
                    {
                        if(isLocalPlayer || !isLocalPlayer) {
                            foreach (Behaviour childCompnent in gameObject.GetComponentsInChildren<Behaviour>()) {
                                if(GetComponent<Animator>()) {
                                    gameObject.GetComponent<Animator>().enabled = true;
                                }
                                childCompnent.enabled = false;
                            }
                            StartCoroutine(removeFromNetwork());
                            Destroy(gameObject, 3);

                        }
                                                      
                    } 
                } 

                IEnumerator removeFromNetwork() {
                    yield return new WaitForSeconds(3f);
                    NetworkServer.Destroy(gameObject);
                }
    }
 

