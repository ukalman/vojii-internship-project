using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDash : IMovement
{
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private PlayerMovement owner;

    private bool isDashing;
    private float dashEndTime;
    public float dashForce = 20f;
    public float dashDuration = 0.5f;

    public void Initialize(Rigidbody playerRigidbody, Transform playerTransform, MonoBehaviour owner)
    {
        this.playerRigidbody = playerRigidbody;
        this.playerTransform = playerTransform;
        this.owner = (PlayerMovement)owner;

        dashEndTime = -1;
        isDashing = false;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && owner.dashPowerUpPickedUp)
        {
            StartDash();
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            isDashing = false;
            playerRigidbody.velocity = Vector3.zero;
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
        playerRigidbody.velocity = dashDirection * dashForce;
        isDashing = true;
        dashEndTime = Time.time + dashDuration;
    }

    private void DashMovement()
    {
        if (isDashing)
        {
            Vector3 dashDirection = playerTransform.forward;
            playerRigidbody.velocity = dashDirection * dashForce;
            isDashing = true;
            dashEndTime = Time.time + dashDuration;


            owner.StartCoroutine(StopDash());
        }
    }



    private IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashDuration);
        playerRigidbody.velocity = Vector3.zero; // stop player's movement instantly
        isDashing = false;
    }
}
