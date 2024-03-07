
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;


public class PlayerCamera : ModuleBase
{
 
    public Transform cameraTransform;
    public Transform playerTransform;
    public Transform visorTransform;

    public Vector3 offset;
    public float sensitivity;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;

    public float currentZoom;
    public float pitch;
    public float yaw;

    public bool fpsCamOn;

    private float xRotation = 0f;

    [SerializeField] private Camera mainCamera;
    
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private float aimFOV = 45f; 
    [SerializeField] private float normalFOV = 60f; 
    [SerializeField] private float fovTransitionDuration = 0.2f; 

    //private void Awake()
    //{
    //    this.priority = 0;
    //}

    private void OnEnable()
    {
        CameraSwitchEvent.OnCameraSwitch += HandleCameraSwitch;
        WeaponAimedEvent.OnWeaponAimed += HandleWeaponAim;
        WeaponUnAimedEvent.OnWeaponUnAimed += HandleWeaponUnAim;
    }

    private void OnDisable()
    {
        CameraSwitchEvent.OnCameraSwitch -= HandleCameraSwitch;
        WeaponAimedEvent.OnWeaponAimed -= HandleWeaponAim;
        WeaponUnAimedEvent.OnWeaponUnAimed -= HandleWeaponUnAim;
    }

    public override IEnumerator IE_Initialize()
    {

        yield return StartCoroutine(base.IE_Initialize());
        this.moduleName = "Player Camera Module";
        this.Priority = 0;
        sensitivity = 300f;
        zoomSpeed = 2f;
        minZoom = 0.1f;
        maxZoom = 15f;

        currentZoom = 5f;
        pitch = 0f;
        yaw = 0f;

        fpsCamOn = false;
        yield return null;

    }

    public override IEnumerator IE_PostInitialize()
    {
        yield return StartCoroutine(base.IE_PostInitialize());
        Vector3 angles = cameraTransform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        yield return null;

    }


    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked; 

    }

    public override IEnumerator IE_Restart()
    {
        yield return StartCoroutine(base.IE_Restart());
        pitch = 0f;
        yaw = 0f;

        cameraTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);


    }


    public override bool Tick()
    {
        if (base.Tick())
        {
            HandleInputForCamera();
            return true;
        }

        return false;


    }

    private void HandleInputForCamera()
    {

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;


        if (fpsCamOn)
        {
            

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            playerTransform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            yaw += mouseX;
            pitch -= mouseY; // mouse y
            pitch = Mathf.Clamp(pitch, -90f, 90f); 
            
            cameraTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            playerTransform.eulerAngles = new Vector3(0f, yaw, 0f);
        }
        
        
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        
 
    }

    public override bool LateTick()
    {
        if (base.LateTick())
        {
            AdjustCameraTransform();
            return true;
        }

        return false;


    }

    private void AdjustCameraTransform()
    {
        if (!fpsCamOn)
        {
            Vector3 desiredPosition = transform.position - cameraTransform.forward * currentZoom + offset;
            RaycastHit hit;

            // If there is something between the camera and the player
            if (Physics.Raycast(transform.position + offset, -cameraTransform.forward, out hit, currentZoom))
            {
                desiredPosition = hit.point;

            }
            cameraTransform.position = desiredPosition;
            cameraTransform.LookAt(transform.position + offset);

        }
        

    }
    
    private void HandleCameraSwitchWrapper(Transform newCameraTransform)
    {
        //StartCoroutine(HandleCameraSwitch(newCameraTransform));
    }



    private void HandleCameraSwitch(Transform newCameraTransform)
    {
        cameraTransform = newCameraTransform;  
        //yield return StartCoroutine(IE_PostInitialize()); // Recalculate angles and positions based on the new cameraTransform
        StartCoroutine(IE_PostInitialize());
        if (newCameraTransform.gameObject.name.Equals("FPS Camera"))
        {
            fpsCamOn = true;
        }
        else fpsCamOn = false;
        
    }
    
    private IEnumerator ChangeFOV(float targetFOV)
    {
        float startFOV = fpsCamera.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < fovTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            fpsCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / fovTransitionDuration);
            yield return null;
        }
        
        fpsCamera.fieldOfView = targetFOV;
    }
    
    private void HandleWeaponAim(AttackState attackState)
    {
        float currentFov = fpsCamera.fieldOfView;
        if (currentFov != aimFOV)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeFOV(aimFOV));
        }
       
    }

    private void HandleWeaponUnAim(AttackState attackState)
    {
        float currentFov = fpsCamera.fieldOfView;
        if (currentFov != normalFOV)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeFOV(normalFOV));
        }
        
    }
}
