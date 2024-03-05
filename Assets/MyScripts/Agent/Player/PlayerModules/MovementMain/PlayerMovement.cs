
using System.Collections;
using System.Collections.Generic;
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

    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 startScale;
    public Transform playerTransform;
    //public Rigidbody Rigidbody;

    public CharacterController playerController;
    private PlayerCamera _cameraModule;
    

    public float MovementSpeed;
    public float maxSpeed;
    public float JumpForce;
    public float SideJumpForce;
    public float DoubleJumpForce;
    public float dashForce;
    public float dashDuration;


    public List<MovementState> DirectionsPressed;
    private Vector3 directionVector = Vector3.zero;
    public bool isJumping;
    public bool canDoubleJump;

    public bool doubleJumpPowerUpPickedUp;
    public bool dashPowerUpPickedUp;
    public bool wallRunPowerUpPickedUp;

    public bool isWallRunning = false;
    public bool wallLeft = false;
    public bool wallRight = false;
    

    private GameObject currentRopeSegment; // The segment the player is currently holding onto
    private bool isHoldingRope = false;
    public float swingForce = 2f;
    private Vector3 currentRopeSegmentCollisionNormal;
    

    public override IEnumerator IE_Initialize()
    {

        yield return StartCoroutine(base.IE_Initialize());
        this.moduleName = "Player Movement Module";
        _cameraModule = Parent.GetModule<PlayerCamera>();
        
        Debug.Log("Hello there, Now we're in the Character controller branch.");

        //Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        DirectionsPressed = new List<MovementState>();
        maxSpeed = 10f;

        isJumping = false;
        canDoubleJump = false;

        doubleJumpPowerUpPickedUp = false;
        dashPowerUpPickedUp = false;
        wallRunPowerUpPickedUp = false;


        dashModule = new MovementDash();
        //dashModule.Initialize(Rigidbody, playerTransform, this);
        dashModule.Initialize(playerController, playerTransform, this);
        
        wallRunningModule = new MovementWallRunning();
        //wallRunningModule.Initialize(Rigidbody, playerTransform, this);
        wallRunningModule.Initialize(playerController, playerTransform, this);

        jumpModule = new MovementJump();
        //jumpModule.Initialize(Rigidbody, playerTransform, this);
        jumpModule.Initialize(playerController, playerTransform, this);

    }

    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());

    }

    public override IEnumerator IE_Restart()
    {
        yield return StartCoroutine(base.IE_Restart());

        playerTransform.position = startPosition;
        playerTransform.rotation = startRotation;
        playerTransform.localScale = startScale;
        isJumping = false;
        canDoubleJump = false;
        doubleJumpPowerUpPickedUp = false;
        dashPowerUpPickedUp = false;
        wallRunPowerUpPickedUp = false;
        DirectionsPressed.Clear();

        isWallRunning = false;
        wallLeft = false;
        wallRight = false;
        //Rigidbody.velocity = Vector3.zero;


    }


    public override bool Tick()
    {
        if (base.Tick())
        {
            
            
            
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            playerController.Move(move * (MovementSpeed * Time.deltaTime));
     
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
            playerTransform.eulerAngles = new Vector3(0f, _cameraModule.yaw, 0f);

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

    private void Move()
    {
        directionVector = Vector3.zero;


        PlayerCamera cameraModule = Parent.GetModule<PlayerCamera>();

        if (cameraModule != null)
        {
            playerTransform.eulerAngles = new Vector3(0f, cameraModule.yaw, 0f);
        }



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
            /*
            if (Rigidbody.velocity.magnitude < maxSpeed)
            {
                Rigidbody.AddForce(directionVector * (MovementSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }
            */
        }



    }


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

