using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerAnimations : MonoBehaviour
{
   private Animator anim;
   private string MOVE = "Move";
   private string VELOCITY_Y = "VelocityY";
   private string CROUCH = "Crouch";
   private string CROUCH_WALK = "CrouchWalk";
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Movement(float magnitude) {
        anim.SetFloat(MOVE, magnitude);
    }
    public void PlayerJump(float velocity) {
        anim.SetFloat(VELOCITY_Y, velocity);
    }
    public void PlayerCrouch(bool iscrouching) {
        anim.SetBool(CROUCH, iscrouching);
    }
    public void PlayerCrouchWalk(float magnitude) {
        anim.SetFloat (CROUCH_WALK, magnitude);
    }
}
