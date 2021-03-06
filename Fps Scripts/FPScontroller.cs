﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class FPScontroller : NetworkBehaviour
{
   private Transform firstPerson_view;
   private Transform firstPerson_Camera;
   private Vector3 firstPerson_view_rotation = Vector3.zero;
   public float walkSpeed = 6.75f;
   public float runSpeed = 20f;
   public float crouchSpeed = 4f;
   public float jumpSpeed = 8f;
   public float gravity = 20f;
   private float speed;

   private bool is_moving, is_grounded, is_crouching;

   private float inputX, inputY;

   private float inputX_Set, inputY_Set;

   private float inputModifyFactor;

   private bool limitDiagonalSpeed = true;

   private float antiBumpFactor = 0.1f;

   private CharacterController charController;

   private Vector3 moveDirection = Vector3.zero;

   public LayerMask groundLayer;

   private float rayDistance;

   private float default_controllerHeight;
   private Vector3 default_CamPos;
   private float camHeight;

   private FPSPlayerAnimations playerAnimations;

   [SerializeField]
   private WeaponManager weapon_Manager;
   private FPSWeapon current_Weapon;
   private float fireRate = 15f;
   private float nextTimeToFire = 0f;

   [SerializeField]
   private WeaponManager handsWeapon_Manager;
   private FPSHandsWeapon current_hands_Weapon;

   public GameObject playerHolder, weaponsHolder;
  
   public GameObject[] weapons_FPS;

   public Camera mainCam;
   public float npcDamage = 25;
   public FPSMouseLooker[] mouseLooks;

   private Color[] playerSkins = new Color[] {
       
       new Color(0,44,255,255),
       new Color(252,208,193,255)
   
   };
   public Renderer playererRendered;
   public GameObject ak47Back;
   public GameObject m4a1Back;

   public FloatVariable isShooting;
   public FloatVariable ammo;
   public GameObject reloadNoification;


   bool isFpsActive = false;
    void Start()
    {
        firstPerson_view = transform.Find("FPS VIEW").transform;
        charController = GetComponent<CharacterController> ();
        speed = walkSpeed;
        is_moving = false;
        isShooting.value = 1;
        reloadNoification = GameObject.Find("ReloadNotification");
        reloadNoification.GetComponent<Animator>().enabled = false; 
        reloadNoification.transform.localScale = Vector3.zero;
        rayDistance = charController.height * 0.5f + charController.radius;
        default_controllerHeight = charController.height;
        default_CamPos = firstPerson_view.localPosition;
        playerAnimations = GetComponent<FPSPlayerAnimations>();
        weapon_Manager.weapons[0].SetActive(true);
        current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();
        handsWeapon_Manager.weapons[0].SetActive(true);
        if(isFpsActive) {
            current_hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon> (); 
        }
               


        // if(isLocalPlayer) {
        //     playerHolder.layer = LayerMask.NameToLayer ("Player");

        //     foreach (Transform child in playerHolder.transform) {
        //         child.gameObject.layer = LayerMask.NameToLayer ("Player");
        //     }
        //     for(int i = 0; i < weapons_FPS.Length; i++) {
        //         weapons_FPS [i].layer = LayerMask.NameToLayer("Player");
        //     }
        //     weaponsHolder.layer = LayerMask.NameToLayer ("Enemy");

        //     foreach(Transform child in weaponsHolder.transform) {
        //         child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        //     }

        // }
        // if(!isLocalPlayer) {
        //     playerHolder.layer = LayerMask.NameToLayer ("Enemy");

        //     foreach (Transform child in playerHolder.transform) {
        //         child.gameObject.layer = LayerMask.NameToLayer ("Enemy");
        //     }
        //     for(int i = 0; i < weapons_FPS.Length; i++) {
        //         weapons_FPS [i].layer = LayerMask.NameToLayer("Enemy");
        //     }
        //     weaponsHolder.layer = LayerMask.NameToLayer ("Player");

        //     foreach(Transform child in weaponsHolder.transform) {
        //         child.gameObject.layer = LayerMask.NameToLayer("Player");
        //     }
        // }

        if(!isLocalPlayer) {
            for(int i = 0; i < mouseLooks.Length; i++) {
                mouseLooks[i].enabled = false;
            }
        }
        mainCam = transform.Find("FPS VIEW").Find("FPS Camera").GetComponent<Camera>();
        mainCam.gameObject.SetActive(false);

        if(!isLocalPlayer) {
            tag = "Enemy";
            for(int i = 0; i < playererRendered.materials.Length; i++) {
                /// RANDOM SKIN IN FUTURE;
            }
        }
    }

    public override void OnStartLocalPlayer() {
        tag = "Player";
    }

    void Update()
    {
        if(isLocalPlayer) {
            if(!mainCam.gameObject.activeInHierarchy) {
                mainCam.gameObject.SetActive(true);
            }
        }
        if(!isLocalPlayer) {
            return;
        }
        PlayerMovement();
    }

    void PlayerMovement() {
        if(Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)) {
            if(Input.GetKey (KeyCode.W)) {
                inputY_Set = 1f;
            } else {
                inputY_Set = -1f;
            }
        } else {
            inputY_Set = 0f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey (KeyCode.D)) {
            if(Input.GetKey (KeyCode.A)) {
                inputX_Set = -1f;
            } else {
                inputX_Set = 1f;
            }
        } else {
            inputX_Set = 0f;
        }

        inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 19f);
        inputX = Mathf.Lerp (inputX, inputX_Set, Time.deltaTime * 19f);

        inputModifyFactor = Mathf.Lerp (inputModifyFactor, (inputY_Set != 0 && inputX_Set != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f, Time.deltaTime * 19f);

        firstPerson_view_rotation = Vector3.Lerp(firstPerson_view_rotation, Vector3.zero, Time.deltaTime * 5f);

        firstPerson_view.localEulerAngles = firstPerson_view_rotation;

        if(is_grounded) {
            /// CALL CROUCH AND SPRINT
            PlayerCrouchingAndSprinting();

            moveDirection = new Vector3 (inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDirection = transform.TransformDirection(moveDirection) * speed;

            //  CALL JUMP HERE
            PlayerJump();
        }

        moveDirection.y -= gravity * Time.deltaTime;

        is_grounded = (charController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        is_moving = charController.velocity.magnitude > 0.15f;
        HandleAnimations();
        SelectWeapon();

    }
    void PlayerCrouchingAndSprinting() {
        if(Input.GetKeyDown(KeyCode.C)) {

            if(!is_crouching) {
                is_crouching = true;
            } else {
                if(CanGetUp()) {
                    is_crouching = false;
                }
            }
            StopCoroutine(MoveCameraCrouch());
            StartCoroutine(MoveCameraCrouch());

        }
        if(is_crouching) {
            speed = crouchSpeed;
            } else {
            if(Input.GetKey(KeyCode.LeftShift)) {
                speed = runSpeed;
            } else {
                speed = walkSpeed;
            }
        }
        playerAnimations.PlayerCrouch(is_crouching);

    }
    bool CanGetUp() {
        Ray groundRay = new Ray (transform.position, transform.up);
        RaycastHit groundHit;

        if(Physics.SphereCast(groundRay, charController.radius + 0.05f, out groundHit, rayDistance, groundLayer)) {
            if(Vector3.Distance(transform.position, groundHit.point) < 2.3f) {
                return false;
            }

        }
        return true;
    }
    IEnumerator MoveCameraCrouch() {
        charController.height = is_crouching ? default_controllerHeight / 1.5f : default_controllerHeight;
        charController.center = new Vector3(0f, charController.height / 2f, 0f);

        camHeight = is_crouching ? default_CamPos.y / 1.5f : default_CamPos.y;

        while(Mathf.Abs(camHeight - firstPerson_view.localPosition.y) > 0.01f) {
            firstPerson_view.localPosition = Vector3.Lerp(firstPerson_view.localPosition, new Vector3(default_CamPos.x, camHeight, default_CamPos.z), Time.deltaTime * 11f);

            yield return null;
        }
    }
    void PlayerJump () {
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(is_crouching) {
                if(CanGetUp()) {
                    is_crouching = false;
                    playerAnimations.PlayerCrouch(is_crouching);
                    StartCoroutine(MoveCameraCrouch());
                    StopCoroutine(MoveCameraCrouch());
                } 
            } else {
                moveDirection.y = jumpSpeed + 7;
            }
        }

    }
    void HandleAnimations() {
        playerAnimations.Movement(charController.velocity.magnitude);
        playerAnimations.PlayerJump(charController.velocity.y);
        if(is_crouching && charController.velocity.magnitude > 0f) {
            playerAnimations.PlayerCrouchWalk(charController.velocity.magnitude);
        }
        //shooting stuffz
        if(isShooting.value == 1) {
                if(Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire) {
                     nextTimeToFire = Time.time + 1f / fireRate;
                        if(is_crouching) {
                            playerAnimations.Shoot(false);
                        } else {
                            playerAnimations.Shoot(true);
                        }

                        if(isLocalPlayer){
                            current_Weapon.Shoot();
                            if(isFpsActive) {
                                    current_hands_Weapon.Shoot();
                            }
                            
                        } else {
                                current_Weapon.Shoot();
                                if(isFpsActive) {
                                    current_hands_Weapon.Shoot();
                                }
                                
                        }

                 } 

        }



        

        if(Input.GetKeyDown(KeyCode.R)) {
            playerAnimations.Reload();
            reloadNoification.transform.localScale = Vector3.zero;
            reloadNoification.GetComponent<Animator>().enabled = false;
            isShooting.value = 1;
            if(isFpsActive) {
                current_hands_Weapon.Reload();
            }
                PlayerPrefs.SetInt("DEAGLE", 20);
                PlayerPrefs.SetInt("AK47", 20);
                PlayerPrefs.SetInt("M4A1", 20);
        }
    }
    void SelectWeapon() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            ak47Back.SetActive(true);
            m4a1Back.SetActive(true);
            ammo.value = PlayerPrefs.GetInt("DEAGLE");

            if(!handsWeapon_Manager.weapons [0].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[0].SetActive(true);
                if(isFpsActive) {
                       current_hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>(); 
                } 
          }
            if(!weapon_Manager.weapons [0].activeInHierarchy) {
                for(int i = 0; i < weapon_Manager.weapons.Length; i++) {
                    weapon_Manager.weapons[i].SetActive(false);
                }

                current_Weapon = null;
                weapon_Manager.weapons[0].SetActive(true);
                current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon> ();
                playerAnimations.ChangeController(true);

            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            ak47Back.SetActive(false);
            m4a1Back.SetActive(true);
            ammo.value = PlayerPrefs.GetInt("AK47");
            if(!handsWeapon_Manager.weapons [1].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[1].SetActive(true);
                if(isFpsActive) {
                    current_hands_Weapon = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();  
                }
                
                }
                if(!weapon_Manager.weapons [1].activeInHierarchy) {
                for(int i = 0; i < weapon_Manager.weapons.Length; i++) {
                    weapon_Manager.weapons[i].SetActive(false);
                }

                current_Weapon = null;
                weapon_Manager.weapons[1].SetActive(true);
                current_Weapon = weapon_Manager.weapons[1].GetComponent<FPSWeapon> ();
                playerAnimations.ChangeController(false);

            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            ak47Back.SetActive(true);
            m4a1Back.SetActive(false);
            ammo.value = PlayerPrefs.GetInt("M4A1");

            if(!handsWeapon_Manager.weapons [2].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[2].SetActive(true);
                if(isFpsActive) {
                     current_hands_Weapon = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();  
                }
               
          }
            if(!weapon_Manager.weapons [2].activeInHierarchy) {
                for(int i = 0; i < weapon_Manager.weapons.Length; i++) {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[2].SetActive(true);
                current_Weapon = weapon_Manager.weapons[2].GetComponent<FPSWeapon> ();
                playerAnimations.ChangeController(false);

            }
        }

    }
    public void TeleportPlayer() {
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
