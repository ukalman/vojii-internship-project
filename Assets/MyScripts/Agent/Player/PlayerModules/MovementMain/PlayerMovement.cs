
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Serialization;


public enum MovementState
{
    Forward,
    Backward,
    Rightward,
    Leftward,
    Jump,
    Dash
}

public class PlayerMovement : AgentModuleBase
{

    private IMovement dashModule;
    private IMovement wallRunningModule;
    private IMovement jumpModule;

    [Header("Start Properties")] 
    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 startScale;
    
    [Header("Player Properties")] 
    public Transform playerTransform;
    //public Rigidbody Rigidbody;

    [FormerlySerializedAs("playerController")] public CharacterController playerCharacterController;
    private PlayerCamera _cameraModule;


    public Vector3 direction = Vector3.zero;
    public float decelerationSpeed = 5f;
    public float MovementSpeed;
    public float maxSpeed;
    
    public List<MovementState> DirectionsPressed;
    private Vector3 directionVector = Vector3.zero;

    
    // Falling with Gravity and Ground
    [Header("Gravity")] 
    public float gravity = -16.677f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Vector3 verticalVelocity;
    public bool isGrounded;

    // Jump
    [Header("Jump Properties")] 
    public float jumpHeight = 3f;
    public float sideJumpHeight = 2f;
    public bool isJumping;
    public bool canDoubleJump;

    
    // Dash
    [Header("Dash Properties")] 
    public float dashForce = 0.5f;
    public float dashDuration = 0.5f;
    
    // Wall Run
    [Header("Wall Run Properties")] 
    public bool isWallRunning = false;
    public bool wallLeft = false;
    public bool wallRight = false;
    
    [Header("PowerUp Properties")] 
    public bool doubleJumpPowerUpPickedUp;
    public bool dashPowerUpPickedUp;
    public bool wallRunPowerUpPickedUp;


    [Header("Rope Swing")]
    private GameObject currentRopeSegment; // The segment the player is currently holding onto
    private bool isHoldingRope = false;
    public float swingForce = 2f;
    private Vector3 currentRopeSegmentCollisionNormal;
    

    public override IEnumerator IE_Initialize()
    {

        yield return StartCoroutine(base.IE_Initialize());
        this.moduleName = "Player Movement Module";
        _cameraModule = Parent.GetModule<PlayerCamera>();

        startPosition = playerTransform.position;
        Debug.Log("Hello there, Now we're in the Character controller branch.");

        //Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //DirectionsPressed = new List<MovementState>();
        maxSpeed = 10f;

        isJumping = false;
        canDoubleJump = false;

        doubleJumpPowerUpPickedUp = false;
        dashPowerUpPickedUp = false;
        wallRunPowerUpPickedUp = false;


        dashModule = new MovementDash();
        //dashModule.Initialize(Rigidbody, playerTransform, this);
        dashModule.Initialize(playerCharacterController, playerTransform, this);
        
        wallRunningModule = new MovementWallRunning();
        //wallRunningModule.Initialize(Rigidbody, playerTransform, this);
        wallRunningModule.Initialize(playerCharacterController, playerTransform, this);

        jumpModule = new MovementJump();
        //jumpModule.Initialize(Rigidbody, playerTransform, this);
        jumpModule.Initialize(playerCharacterController, playerTransform, this);

    }

    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());

    }

    public override IEnumerator IE_Restart()
    {
        Debug.Log("Player Movement Restart is working!");

        // Disable the CharacterController before resetting position and rotation
        playerCharacterController.enabled = false;

        // Reset player transform to start positions and rotations
        playerTransform.position = startPosition;
        playerTransform.rotation = startRotation;
        playerTransform.localScale = startScale;

        // Reset movement and ability flags
        isJumping = false;
        canDoubleJump = false;
        doubleJumpPowerUpPickedUp = false;
        dashPowerUpPickedUp = false;
        wallRunPowerUpPickedUp = false;
    
        // Reset wall running state
        isWallRunning = false;
        wallLeft = false;
        wallRight = false;

        // Reset vertical velocity to ensure no residual falling/movement speeds
        verticalVelocity = Vector3.zero;

        // Re-enable the CharacterController after the reset
        playerCharacterController.enabled = true;

        // Optionally wait for the end of the frame to ensure all physics and other updates are processed
        yield return new WaitForEndOfFrame();

    }


    public override bool Tick()
    {
        if (base.Tick())
        {
            CheckMove();
            jumpModule.Tick();
            dashModule.Tick();
            wallRunningModule.Tick();

            return true;
        }

        return false;



        /*

        if (base.Tick())
        {
            CheckRopeAction();


            if (!isHoldingRope)
            {
                CheckKeyPress();

                CheckKeyRelease();

                dashModule.Tick();

                if (wallRunPowerUpPickedUp) wallRunningModule.Tick();

                jumpModule.Tick();
            } else
            {
                //transform.position = currentSegment.transform.position + Vector3.down * hangingOffset;
                //playerTransform.position = currentRopeSegment.transform.position + Vector3.right;
                playerTransform.position = currentRopeSegment.transform.position + currentRopeSegmentCollisionNormal;


                if (Input.GetKeyDown(KeyCode.W))
                {
                    Swing("Forward"); // Swing forward
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    Swing("Backward"); // Swing backward
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    Swing("Rightward"); // Swing rightward
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    Swing("Leftward"); // Swing leftward
                }

            }

            return true;

        }

        return false;
        */



    }

  
    public override bool FixedTick()
    {

        if (base.FixedTick())
        {
            //playerTransform.eulerAngles = new Vector3(0f, _cameraModule.yaw, 0f);
            Move();
            ApplyGravity();
            
            jumpModule.FixedTick();
            dashModule.FixedTick();
            wallRunningModule.FixedTick();
            
            return true;
        }

        return false;


        /*
        if (base.FixedTick())
        {


            Move();

            if (!isHoldingRope)
            {

            }
            dashModule.FixedTick();

            if(wallRunPowerUpPickedUp) wallRunningModule.FixedTick();

            jumpModule.FixedTick();

            return true;
        }

        return false;
        */


    }
    
    private void CheckKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!DirectionsPressed.Contains(MovementState.Forward))
            {
                DirectionsPressed.Add(MovementState.Forward);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!DirectionsPressed.Contains(MovementState.Backward))
            {
                DirectionsPressed.Add(MovementState.Backward);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!DirectionsPressed.Contains(MovementState.Leftward))
            {
                DirectionsPressed.Add(MovementState.Leftward);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!DirectionsPressed.Contains(MovementState.Rightward))
            {
                DirectionsPressed.Add(MovementState.Rightward);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isWallRunning)
        {
            if (!DirectionsPressed.Contains(MovementState.Jump))
            {
                DirectionsPressed.Add(MovementState.Jump);
            }
        }



        if (Input.GetKeyDown(KeyCode.Space) && isJumping && canDoubleJump && !isWallRunning && doubleJumpPowerUpPickedUp)
        {
            if (!DirectionsPressed.Contains(MovementState.Jump))
            {
                DirectionsPressed.Add(MovementState.Jump);
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && dashPowerUpPickedUp)
        {
            if (!DirectionsPressed.Contains(MovementState.Dash))
            {
                DirectionsPressed.Add(MovementState.Dash);
            }
        }

    }

    private void CheckKeyRelease()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            DirectionsPressed.Remove(MovementState.Forward);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DirectionsPressed.Remove(MovementState.Backward);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            DirectionsPressed.Remove(MovementState.Leftward);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            DirectionsPressed.Remove(MovementState.Rightward);
        }


    }

    
    
    private void CheckMove()
    {
        float x_fps = Input.GetAxis("Horizontal");
        float z_fps = Input.GetAxis("Vertical");
    
        float horizontal_tps = Input.GetAxisRaw("Horizontal");
        float vertical_tps = Input.GetAxisRaw("Vertical");

        Vector3 targetDirection = Vector3.zero;

        if (!_cameraModule.fpsCamOn)
        {
            targetDirection = (transform.right * horizontal_tps + transform.forward * vertical_tps).normalized;
        }
        else
        {
            targetDirection = transform.right * x_fps + transform.forward * z_fps;
        }

        // Check if there is any input to set the target direction
        if (Mathf.Abs(x_fps) > 0.01f || Mathf.Abs(z_fps) > 0.01f || Mathf.Abs(horizontal_tps) > 0.01f || Mathf.Abs(vertical_tps) > 0.01f)
        {
            direction = targetDirection;
        }
        else
        {
            // Gradually reduce 'direction' to zero if there is no input
            direction = Vector3.MoveTowards(direction, Vector3.zero, decelerationSpeed * Time.deltaTime);
        }
        
    }

    private void Move()
    {
        playerCharacterController.Move(direction * (MovementSpeed * Time.deltaTime));
        Debug.Log("Current move direction: x: " + direction.x + ", y: " + direction.y + ", z: " + direction.z);
    }
    
    private void ApplyGravity()
    {
        Debug.Log("is grounded: " + isGrounded);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity.y < 0)
        {
            canDoubleJump = false;
            verticalVelocity.x = 0f;
            verticalVelocity.z = 0f;
            verticalVelocity.y = -2f;
        }
        
        verticalVelocity.y += gravity * Time.deltaTime;

        playerCharacterController.Move(verticalVelocity * Time.deltaTime);

    }

    
    /*
    private void Move()
    {
        directionVector = Vector3.zero;

        foreach (var direction in DirectionsPressed)
        {
            switch (direction)
            {
                case MovementState.Forward:
                    directionVector += playerTransform.forward;
                    break;
                case MovementState.Backward:
                    directionVector -= playerTransform.forward;
                    break;
                case MovementState.Leftward:
                    directionVector -= playerTransform.right;
                    break;
                case MovementState.Rightward:
                    directionVector += playerTransform.right;
                    break;
            }
        }

        if (directionVector != Vector3.zero)
        {
            directionVector.Normalize();
           
        }

    }
    */

    void CheckRopeAction()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentRopeSegment != null && !isHoldingRope)
        {
            GrabRope();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isHoldingRope) // Use space key to release
        {
            ReleaseRope();
        }
    }

    void GrabRope()
    {
        isHoldingRope = true;
        //Rigidbody.isKinematic = true; // Disable physics-driven movement
        //Rigidbody.useGravity = false;
    }

    void ReleaseRope()
    {
        float forceMultiplier = 1f;
        isHoldingRope = false;
        // Vector3 lastSegmentVelocity = currentRopeSegment.GetComponent<Rigidbody>().velocity;
        // Rigidbody.useGravity = true;
        // Rigidbody.velocity = lastSegmentVelocity;

        currentRopeSegment = null;

    }
    

    void Swing(string direction)
    {
        Rigidbody rb = currentRopeSegment.GetComponent<Rigidbody>();
        // Apply a force in the direction of the player's forward vector times the input direction
        Vector3 forceDirection = Vector3.zero;
        switch (direction)
        {
            case "Forward":
                forceDirection = transform.forward;
                break;
            case "Backward":
                forceDirection = -transform.forward;
                break;
            case "Rightward":
                forceDirection = transform.right;
                break;
            case "Leftward":
                forceDirection = -transform.right;
                break;
            default:
                break;
        }
        
        rb.AddForce(forceDirection * swingForce, ForceMode.Impulse);
    }

    public void HandleCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            return;
        }

        if (other.gameObject.CompareTag("RopeSegment"))
        {
            Debug.Log("Rope Segment Parent: " + other.transform.parent.gameObject.name);
            if (currentRopeSegment == null || other.transform.parent.gameObject != currentRopeSegment.transform.parent.gameObject)
            {
                currentRopeSegmentCollisionNormal = other.contacts[0].normal; // Get the normal of the first contact point
                
                currentRopeSegment = other.transform.gameObject;
                
            }
            
        }


        if (isJumping)
        {
            isJumping = false;
            canDoubleJump = false;

        }

        if (other.gameObject.CompareTag("Ground"))
        {

            if (other.impulse.y > 8f)
            {
                PlayerAudio playerAudio = Parent.GetModule<PlayerAudio>();
                PlayerEffects playerEffects = Parent.GetModule<PlayerEffects>();

                if (playerAudio != null)
                {
                    playerAudio.PlayerAudioState = AudioState.Land;
                }

                if (playerEffects != null)
                {
                    playerEffects.LandingImpulse = other.impulse;
                    playerEffects.PlayerEffectState = EffectState.Land;
                }

              

            }


        }
    }

    public void HandleCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("RopeSegment"))
        {
            currentRopeSegment = null;
        }
    }

}

