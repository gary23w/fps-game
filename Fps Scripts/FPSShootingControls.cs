using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShootingControls : MonoBehaviour
{

    private Camera mainCam;
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;


    [SerializeField]
    private GameObject concrete_Impact;

    void Start()
    {
        mainCam = Camera.main;
        
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
                Instantiate(concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

    }
}
