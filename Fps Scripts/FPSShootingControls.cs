using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShootingControls : NetworkBehaviour
{

    private Camera mainCam;
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;


    [SerializeField]
    private GameObject concrete_Impact, blood_Impact;

    public float damageAmount = 25f;

    void Start()
    {
        mainCam = transform.Find("FPS VIEW").Find("FPS Camera").GetComponent<Camera>();
        
    }

    void Update()
    {
        Shoot();   
    }

    void Shoot() {
        if(Input.GetMouseButtonDown (0) && Time.time > nextTimeToFire) {
            nextTimeToFire = Time.time + 1f / fireRate;

            RaycastHit hit;

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit)) {
                // print("target hit" + hit.collider.gameObject.name);
                // print(hit.transform.position);
                // print(hit.point);
                if(hit.transform.tag == "Enemy") {
                    CmdDealDamage(hit.transform.gameObject, hit.point, hit.normal);
                } else {
                    Instantiate(concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }

    }
    [Command]
    void CmdDealDamage(GameObject obj, Vector3 pos, Vector3 rotation) {
        obj.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        Instantiate(blood_Impact, pos, Quaternion.LookRotation(rotation));

    }
}
