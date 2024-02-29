using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWallRunning : IMovement
{
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private PlayerMovement owner;

    private LayerMask wallLayer;

    private RaycastHit leftWallhit;
    private RaycastHit rightWallHit;


    private bool jumpToLeft;
    private bool jumpToRight;

    public void Initialize(Rigidbody playerRigidbody, Transform playerTransform, MonoBehaviour owner)
    {
        this.playerRigidbody = playerRigidbody;
        this.playerTransform = playerTransform;
        this.owner = (PlayerMovement)owner;

        wallLayer = LayerMask.GetMask("Wall");

        jumpToLeft = false;
        jumpToRight = false;

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
        owner.wallLeft = Physics.Raycast(playerTransform.position, -playerTransform.right, out leftWallhit, 0.8f, wallLayer);

      

        if ((owner.wallLeft || owner.wallRight) && owner.DirectionsPressed.Contains(MovementState.Forward))
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
            owner.GetParent().GetModule<PlayerAudio>().PlayerAudioState = AudioState.Jump;
            if (owner.wallRight)
            {
                jumpToLeft = true;
   
            }
            else
            {
                jumpToRight = true;

            }

   
        }


    }


    private void StartWallRun()
    {

        owner.isWallRunning = true;
        owner.isJumping = false;

      

    }

    private void WallRunningMovement()
    {
        if (owner.isWallRunning)
        {
            playerRigidbody.useGravity = false;
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, -2f, playerRigidbody.velocity.z);

            // get the wall normal broski
            //Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallhit.normal;
            Vector3 wallNormal = owner.wallRight ? rightWallHit.normal : leftWallhit.normal;

            // cross product to get wallforward
            Vector3 wallForward = Vector3.Cross(wallNormal, playerTransform.up);


            playerRigidbody.AddForce(wallForward, ForceMode.Force);

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
        }





    }

    private void StopWallRun()
    {
        //owner.setIsWallRunning(false);
        owner.isWallRunning = false;
        //isWallRunning = false;
        playerRigidbody.useGravity = true;
    }

    private void JumpToLeft()
    {

        if (!owner.isJumping)
        {
            playerRigidbody.AddForce(Vector3.up * owner.JumpForce + (-playerTransform.right.normalized * owner.SideJumpForce), ForceMode.Impulse);
            //isJumping = true;
            //owner.setIsJumping(true);
            owner.isJumping = true;
        }
    }

    private void JumpToRight()
    {
        if (!owner.isJumping)
        {
            playerRigidbody.AddForce(Vector3.up * owner.JumpForce + (playerTransform.right.normalized * owner.SideJumpForce), ForceMode.Impulse);
            //isJumping = true;
            //owner.setIsJumping(true);
            owner.isJumping = true;
        }
    }
}
