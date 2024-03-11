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
            if (Input.GetButtonDown("Jump"))
            {
                if (owner.isGrounded)
                {
                    Debug.Log("Jumping!");

                    // Calculate the jump force for the y-axis
                    owner.verticalVelocity.y = Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
            
                    // Retain the player's current forward momentum when jumping
                    Vector3 currentMovementDirection = playerController.velocity;
                    float currentSpeed = playerController.velocity.magnitude;
                    
                    // Apply a portion of the current movement speed to the jump. Adjust the multiplier as needed.
                    owner.verticalVelocity.x = owner.direction.x * currentSpeed * 2.5f;
                    owner.verticalVelocity.z = owner.direction.z * currentSpeed * 2.5f;

                    owner.canDoubleJump = true; 
                } 
                else if (owner.canDoubleJump && !owner.isGrounded && owner.doubleJumpPowerUpPickedUp)
                {
                    Debug.Log("Double Jumping!");
                    owner.verticalVelocity.y = Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
                    owner.canDoubleJump = false;

                    // For double jump, consider if you want to reset or maintain horizontal momentum
                    // This example maintains the current horizontal momentum without adding more
                }
            }
        }
        
     
    }
    
}
