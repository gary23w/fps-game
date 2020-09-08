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



    void Start()
    {
        firstPerson_view = transform.Find("FPS VIEW").transform;
        charController = GetComponent<CharacterController> ();
        speed = walkSpeed;
        is_moving = false;

        rayDistance = charController.height * 0.5f + charController.radius;
        default_controllerHeight = charController.height;
        default_CamPos = firstPerson_view.localPosition;
        
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
                    StartCoroutine(MoveCameraCrouch());
                    StopCoroutine(MoveCameraCrouch());
                } 
            } else {
                moveDirection.y = jumpSpeed;
            }
        }

    }
}
