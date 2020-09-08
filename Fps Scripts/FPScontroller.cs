using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPScontroller : MonoBehaviour
{
   private Transform firstPerson_view;
   private Transform firstPerson_Camera;
   private Vector3 firstPerson_view_rotation = Vector3.zero;
   public float walkSpeed = 6.75f;
   public float runSpeed = 10f;
   public float crouchSpeed = 4f;
   public float jumpSpeed = 8f;
   public float gravity = 20f;

   private float speed;

   private bool is_moving, is_grounded, is_crouching;

   private float inputX, inputY;

   private float inputX_Set, inputY_Set;

   private float inputModifyFactor;

   private bool limitDiagonalSpeed = true;

   private float antiBumpFactor = 0.75f;

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
    void Start()
    {
        firstPerson_view = transform.Find("FPS VIEW").transform;
        charController = GetComponent<CharacterController> ();
        speed = walkSpeed;
        is_moving = false;

        rayDistance = charController.height * 0.5f + charController.radius;
        default_controllerHeight = charController.height;
        default_CamPos = firstPerson_view.localPosition;

        playerAnimations = GetComponent<FPSPlayerAnimations>();

        weapon_Manager.weapons[0].SetActive(true);
        current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

        handsWeapon_Manager.weapons[0].SetActive(true);
        current_hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon> ();        
    }

    void Update()
    {
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
                moveDirection.y = jumpSpeed;
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

        if(Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire) {
            nextTimeToFire = Time.time + 1f / fireRate;

            if(is_crouching) {
                playerAnimations.Shoot(false);
            } else {
                playerAnimations.Shoot(true);
            }

            current_Weapon.Shoot();
            current_hands_Weapon.Shoot();
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            playerAnimations.Reload();
            current_hands_Weapon.Reload();
        }
    }
    void SelectWeapon() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            if(!handsWeapon_Manager.weapons [0].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[0].SetActive(true);
                current_hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();  
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
            if(!handsWeapon_Manager.weapons [1].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[1].SetActive(true);
                current_hands_Weapon = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();  
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
            if(!handsWeapon_Manager.weapons [2].activeInHierarchy) {
                for(int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_hands_Weapon = null;
                handsWeapon_Manager.weapons[2].SetActive(true);
                current_hands_Weapon = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();  
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
}
