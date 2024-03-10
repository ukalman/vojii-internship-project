using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWallRunning : IMovement
{
      private CharacterController playerController;
    private Transform playerTransform;
    private PlayerMovement owner;

    private LayerMask wallLayer;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool jumpToLeft;
    private bool jumpToRight;

    public void Initialize(CharacterController playerController, Transform playerTransform, MonoBehaviour owner)
    {
        this.playerController = playerController;
        this.playerTransform = playerTransform;
        this.owner = (PlayerMovement)owner;

        wallLayer = LayerMask.GetMask("Wall");
    }

    public void Tick()
    {
        CheckWallRun();
        CheckWallJump();
    }

    public void FixedTick()
    {
        WallRunningMovement();
    }

    private void CheckWallRun()
    {
        owner.wallRight = Physics.Raycast(playerTransform.position, playerTransform.right, out rightWallHit, 0.8f, wallLayer);
        owner.wallLeft = Physics.Raycast(playerTransform.position, -playerTransform.right, out leftWallHit, 0.8f, wallLayer);

        if ((owner.wallLeft || owner.wallRight) && !owner.isGrounded)
        {
            StartWallRun();
        }
        else
        {
            StopWallRun();
        }
    }

    private void CheckWallJump()
    {
        if (owner.isWallRunning && Input.GetKeyDown(KeyCode.Space))
        {
            if (owner.wallRight)
            {
                jumpToLeft = true;
            }
            else if (owner.wallLeft)
            {
                jumpToRight = true;
            }
        }
    }

    private void StartWallRun()
    {
        owner.isWallRunning = true;
        owner.verticalVelocity.y = -2f; // Apply a slight downward force
        owner.isJumping = false;
    }

    private void WallRunningMovement()
    {
        if (owner.isWallRunning)
        {
            Vector3 wallNormal = owner.wallRight ? rightWallHit.normal : leftWallHit.normal;
            Vector3 wallForward;

            if (owner.wallRight)
            {
                // For a wall on the right, cross product of wall normal and up vector should give forward direction
                wallForward = Vector3.Cross(Vector3.up, wallNormal);
            }
            else
            {
                // For a wall on the left, cross product of up vector and wall normal should give forward direction
                wallForward = Vector3.Cross(wallNormal, Vector3.up);
            }

            // Normalize the wallForward vector to ensure it has a unit length
            wallForward = wallForward.normalized;

            playerController.Move(wallForward * (owner.MovementSpeed * Time.deltaTime));

            if (jumpToRight)
            {
                JumpToRight();
                owner.isJumping = false;
                jumpToRight = false;
            }

            else if (jumpToLeft)
            {
                JumpToLeft();
                owner.isJumping = false;
                jumpToLeft = false;
            }
            
            // Jumping logic (if applicable) remains the same
            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpDirection = owner.wallRight ? -playerTransform.right : playerTransform.right;
                jumpDirection += Vector3.up; // Adjust this vector to change the jump direction and strength
                owner.verticalVelocity = jumpDirection * owner.JumpForce;
                StopWallRun(); // Stop wall running when the player jumps off
            }
            */
        }
    }

    private void StopWallRun()
    {
        owner.isWallRunning = false;
    }

    private void JumpToLeft()
    {
        if (owner.isWallRunning && owner.wallRight)
        {
            Debug.Log("Wall Jumping to Left!");
            // Include wall normal direction in jump
            Vector3 jumpDirection = (-playerTransform.right + Vector3.up).normalized + rightWallHit.normal;
            owner.verticalVelocity = jumpDirection * Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
            // Immediately apply the lateral movement to ensure the jump feels responsive
            //playerController.Move(jumpDirection * 0.1f); // Adjust as needed
            StopWallRun(); // Stop wall running after the jump
        }
    }

    private void JumpToRight()
    {
        if (owner.isWallRunning && owner.wallLeft)
        {
            Debug.Log("Wall Jumping to Right!");
            // Include wall normal direction in jump
            Vector3 jumpDirection = (playerTransform.right + Vector3.up).normalized + leftWallHit.normal;
            owner.verticalVelocity = jumpDirection * Mathf.Sqrt(owner.jumpHeight * -2f * owner.gravity);
            // Immediately apply the lateral movement to ensure the jump feels responsive
            //playerController.Move(jumpDirection * 0.1f); // Adjust as needed
            StopWallRun(); // Stop wall running after the jump
        }
    }
}
