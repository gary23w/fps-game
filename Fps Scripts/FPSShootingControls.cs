using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSShootingControls : NetworkBehaviour
{

    private Camera mainCam;
    private float fireRate = 25f;
    private float nextTimeToFire = 0f;
    float ammo = 0;

    public FloatVariable ammoFloat;


    [SerializeField]
    private GameObject concrete_Impact, blood_Impact;


    [SyncVar]
    public float damageAmount = 50f;
    public GameObject[] muzzleFlash;

    public FloatVariable isShooting;

    public GameObject reloadNotification;

    void Start()
    {
        mainCam = transform.Find("FPS VIEW").Find("FPS Camera").GetComponent<Camera>(); 
        ammo += 20;   
        reloadNotification = GameObject.Find("ReloadNotification");    
        reloadNotification.GetComponent<Animator>().enabled = false;
    }

    void Update()
    {
            Cmd_Shooting();  
            ammoFloat.value = ammo;     
    }
    public void Cmd_Shooting() {
        
    if(Input.GetMouseButtonDown (0) && Time.time > nextTimeToFire) {
        if (ammo == 0) {
            DisableMuzzleFlashes(true);
            reloadNotification.GetComponent<Animator>().enabled = true;
            reloadNotification.transform.localScale = new Vector3(1,1,1);
            isShooting.value = 0;
            Debug.Log("you must reload");
            return;
        }
        if(ammo <= 20) {
            DisableMuzzleFlashes(false);
            ammo--;
            nextTimeToFire = Time.time + 1f / fireRate;
            RaycastHit hit;
            

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit)) {
                // print("target hit" + hit.collider.gameObject.name);
                // print(hit.transform.position);
                // print(hit.point);
                IEntity npc = hit.transform.GetComponent<IEntity>();
                if(hit.transform.tag == "Enemy") {
                    CmdDealDamage(hit.transform.gameObject, hit.point, hit.normal);
                    print("target hit" + hit.collider.gameObject.name);
                }  if(hit.transform.tag == "Zombie") {
                        //Apply damage to NPC
                        print("target hit" + hit.collider.gameObject.name); 
                        Instantiate(blood_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                        npc.Cmd_ApplyDamage(damageAmount);
                   
                } if(hit.transform.tag == "Player") {
                     Debug.Log("3rd person view fix");
                     return;                   
                } else {
                    Instantiate(concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        } 
    } 

    }

    [Command]
    void CmdDealDamage(GameObject obj, Vector3 pos, Vector3 rotation) {
        obj.GetComponent<PlayerHealth>().Cmd_ApplyDamage(damageAmount);
        Instantiate(blood_Impact, pos, Quaternion.LookRotation(rotation));


    }
    
            public void ReloadFPScontroller() {
                ammo = 20;
            }

            public void DisableMuzzleFlashes(bool isDisabled) {
                if(isDisabled = true) {
                for(int i = 0; i > 10; i++) {
                    muzzleFlash[i].SetActive(false);
                }
                } else {
                for(int i = 0; i > 10; i++) {
                    muzzleFlash[i].SetActive(true);
                }
                }
            }
}


