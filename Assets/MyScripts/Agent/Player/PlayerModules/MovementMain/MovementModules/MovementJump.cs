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

        //canJump = false;
        //canDoubleJump = false;
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
            } else if (owner.canDoubleJump && !owner.isGrounded && owner.doubleJumpPowerUpPickedUp)
            {
                Debug.Log("Double Jumping!");
                owner.verticalVelocity.y = Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
                owner.canDoubleJump = false;
            }
        }
        
     
    }
    
}
