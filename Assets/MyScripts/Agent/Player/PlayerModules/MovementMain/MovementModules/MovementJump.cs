using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementJump : IMovement
{
    //private Rigidbody playerRigidbody;
    private CharacterController playerController;
    private Transform playerTransform;
    private PlayerMovement owner;

    private bool canJump;
    private bool canDoubleJump;

    public void Initialize(CharacterController playerController, Transform playerTransform, MonoBehaviour owner)
    {
        this.playerController = playerController;
        this.playerTransform = playerTransform;
        this.owner = (PlayerMovement)owner;

        canJump = false;
        canDoubleJump = false;
    }

    public void Tick()
    {
        //CheckJump();
        Jump();
    }

    public void FixedTick()
    {
        // if (canJump)
        // {
        //     Jump();
        // }
        //
        // if (canDoubleJump)
        // {
        //     Jump();
        // }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (owner.isGrounded)
            {
                Debug.Log("Jumping!");
                owner.verticalVelocity.y = Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
                owner.canDoubleJump = true; 
            } else if (owner.canDoubleJump && !owner.isGrounded)
            {
                Debug.Log("Double Jumping!");
                owner.verticalVelocity.y = Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
                owner.canDoubleJump = false;
            }
        }
        
     
    }
    

    /*
    private void Jump()
    {
        if (!owner.isJumping)
        {
            playerRigidbody.AddForce(Vector3.up * owner.JumpForce, ForceMode.Impulse);
            owner.isJumping = true;
            //owner.canDoubleJump = true;
            canJump = false;

        }
        else
        {
            if (canDoubleJump)
            {

                playerRigidbody.AddForce(Vector3.up * owner.DoubleJumpForce, ForceMode.Impulse);
                owner.canDoubleJump = false;
                canDoubleJump = false;
            }
        }


    }
    */

    /*
    
    private void CheckJump()
    {
        if ((owner.DirectionsPressed.Contains(MovementState.Jump) && !owner.isJumping))
        {
            owner.GetParent().GetModule<PlayerAudio>().PlayerAudioState = AudioState.Jump;
            canJump = true;
            if (owner.doubleJumpPowerUpPickedUp)
            {
                owner.canDoubleJump = true;
            }
            
            owner.DirectionsPressed.Remove(MovementState.Jump);
        }
        else if (owner.DirectionsPressed.Contains(MovementState.Jump) && owner.isJumping && owner.doubleJumpPowerUpPickedUp)
        {
            owner.GetParent().GetModule<PlayerAudio>().PlayerAudioState = AudioState.Jump;
            canDoubleJump = true;
            owner.DirectionsPressed.Remove(MovementState.Jump);
        }
    }
    */
}
