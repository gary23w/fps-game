using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSPlayerAnimations : NetworkBehaviour
{
   private Animator anim;
   private string MOVE = "Move";
   private string VELOCITY_Y = "VelocityY";
   private string CROUCH = "Crouch";
   private string CROUCH_WALK = "CrouchWalk";

   private string STAND_SHOOT = "StandShoot";

   private string CROUCH_SHOOT = "CrouchShoot";
   private string RELOAD = "Reload";

   public RuntimeAnimatorController animController_Pistol, animController_MachineGun;

   private NetworkAnimator networkAnim;

   public FPSShootingControls ammo;
    void Awake()
    {
        anim = GetComponent<Animator>();
        networkAnim = GetComponent<NetworkAnimator>();
        ammo.GetComponent<FPSShootingControls>();
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
    public void Shoot(bool isStanding) {
                if(isStanding) {
                    anim.SetTrigger(STAND_SHOOT);
                    networkAnim.SetTrigger(STAND_SHOOT);

                } else {
                    anim.SetTrigger(CROUCH_SHOOT);
                            networkAnim.SetTrigger(CROUCH_SHOOT);
                }
        
    }
    public void Reload() {
        anim.SetTrigger(RELOAD);
                 networkAnim.SetTrigger(RELOAD);
                     ammo.ReloadFPScontroller();
    }

    public void ChangeController(bool isPistol) {
        if(isPistol) {
            anim.runtimeAnimatorController = animController_Pistol;
        } else {
            anim.runtimeAnimatorController = animController_MachineGun;
        }
    }
}
