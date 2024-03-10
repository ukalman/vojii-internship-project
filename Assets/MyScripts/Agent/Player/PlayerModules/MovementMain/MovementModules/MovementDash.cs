using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDash : IMovement
{
    
    private CharacterController playerController;
    private Transform playerTransform;
    private PlayerMovement owner;

    private bool isDashing;
    private float dashEndTime;
    private float dashCooldown = 2f; // Duration of dash cooldown in seconds
    private float nextDashTime = 0f; // Time when player is allowed to dash again

    public void Initialize(CharacterController playerController, Transform playerTransform, MonoBehaviour owner)
    {
        this.playerController = playerController;
        this.playerTransform = playerTransform;
        this.owner = (PlayerMovement)owner;

        dashEndTime = -1;
        isDashing = false;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= nextDashTime && owner.dashPowerUpPickedUp)
        {
            StartDash();
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            isDashing = false;
            nextDashTime = Time.time + dashCooldown; // Set the next time the player is allowed to dash
        }

       
    }

    public void FixedTick()
    {
        DashMovement();
    }

    private void StartDash()
    {
        owner.GetParent().GetModule<PlayerAudio>().PlayerAudioState = AudioState.Dash;
        Vector3 dashDirection = playerTransform.forward;
        playerController.Move(dashDirection * owner.dashForce * Time.deltaTime); // Ensure you multiply by Time.deltaTime for frame rate independence
        isDashing = true;
        dashEndTime = Time.time + owner.dashDuration;
    }

    private void DashMovement()
    {
        if (isDashing)
        {
            Vector3 dashDirection = playerTransform.forward;
            playerController.Move(dashDirection * owner.dashForce * Time.deltaTime); // Ensure you multiply by Time.deltaTime for frame rate independence
            // The `isDashing = true;` and setting `dashEndTime` here seem redundant since they are already set in StartDash(),
            // Consider removing them from here if they don't serve another purpose
        }
    }
}
