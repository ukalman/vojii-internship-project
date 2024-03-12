using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    [Header("Player")]
    public GameObject Player;
    public PlayerAttack playerAttack; // Reference to the PlayerAttack script
    
    [Header("Camera")]
    public Camera mainCamera; 
    public Camera fpsCamera; 

    [Header("Controllers")]
    public ModularObject playerController;
    public ModularObject powerupController;

    
    [Header("Weapon UI")]
    public GameObject pistolUI; // Reference to Pistol panel
    public GameObject katanaUI;
    public GameObject bombUI;
    
    
    
    private DateTime timeStamp;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fpsCamera.transform.SetParent(Player.transform);
        fpsCamera.transform.localPosition = new Vector3(0, 0, 0); // Adjust this to your desired offset
        fpsCamera.transform.localRotation = Quaternion.identity; // Resets rotation to face forward relative to the player
        //mainCamera.transform.SetParent(Player.transform);
        pistolUI.SetActive(false);
        katanaUI.SetActive(false);
        bombUI.SetActive(false);
    }


    private void OnEnable()
    {
        ModuleEventManager.Instance.Subscribe<BeforeModuleStateChangedEvent>(OnBeforeModuleStateChange);
        ModuleEventManager.Instance.Subscribe<AfterModuleStateChangedEvent>(OnAfterModuleStateChange);

        WeaponEquippedEvent.OnWeaponEquipped += HandleWeaponEquipmentUI;

        mainCamera.enabled = true;
        fpsCamera.enabled = false;
    }

    private void OnDisable()
    {
        ModuleEventManager.Instance.Unsubscribe<BeforeModuleStateChangedEvent>(OnBeforeModuleStateChange);
        ModuleEventManager.Instance.Unsubscribe<AfterModuleStateChangedEvent>(OnAfterModuleStateChange);

        WeaponEquippedEvent.OnWeaponEquipped -= HandleWeaponEquipmentUI;
    }

  
    private void Start()
    {
        
    }


    private void Update()
    {
        timeStamp = DateTime.Now;
        UpdateUI();
        CheckForReset();
        CheckForCameraSwitch();

        if (fpsCamera.enabled)
        {
            mainCamera.transform.eulerAngles = fpsCamera.transform.eulerAngles;
        }
    
    }

    private void UpdateUI()
    {
        /*
        if (playerAttack != null && ammoText != null)
        {
            ammoText.text = "Ammo: x" + playerAttack.pistolAmmo.ToString();
        }
        */
    }


    private void CheckForCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            // Toggle cameras
            mainCamera.enabled = !mainCamera.enabled;
            fpsCamera.enabled = !fpsCamera.enabled;

            Debug.Log("Main camera enabled: " + mainCamera.enabled);
            Debug.Log("Fps camera enabled: " + fpsCamera.enabled);

            // Broadcast the camera switch event with the new active camera transform
            Transform newActiveCameraTransform = fpsCamera.enabled ? fpsCamera.transform : mainCamera.transform;
            CameraSwitchEvent.BroadcastCameraSwitch(newActiveCameraTransform);
        }
    }

    private void HandleWeaponEquipmentUI(AttackState attackState)
    {
        if(attackState == AttackState.Pistol)
        {
            pistolUI.SetActive(true);
        } else pistolUI.SetActive(false);

        if (attackState == AttackState.Katana)
        {
            katanaUI.SetActive(true);
        }
        else katanaUI.SetActive(false);

        if (attackState == AttackState.Bomb)
        {
            bombUI.SetActive(true);
        }
        else bombUI.SetActive(false);

    }

  

    void CheckForReset()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("[GameManager] " + timeStamp.ToString() + " Resetting all modules.");
            StartCoroutine(playerController.IE_ResetAllModules());
            StartCoroutine(powerupController.IE_ResetAllModules());
        }
    }

    public void ActivatePowerUp(string powerupType)
    {
        var playerMovementModule = playerController.GetModule<PlayerMovement>();
        if (playerMovementModule != null)
        {
            switch (powerupType)
            {
                case "Double Jump Power Up":
                    playerMovementModule.doubleJumpPowerUpPickedUp = true;
                    Debug.Log("[GameManager] " + timeStamp.ToString() + " Double Jump Power-Up Activated.");
                    break;
                case "Dash Power Up":
                    playerMovementModule.dashPowerUpPickedUp = true;
                    Debug.Log("[GameManager] " + timeStamp.ToString() + " Dash Power-Up Activated.");
                    break;
                case "Wall Run Power Up":
                    playerMovementModule.wallRunPowerUpPickedUp = true;
                    Debug.Log("[GameManager] " + timeStamp.ToString() + " Wall Run Power-Up Activated.");
                    break;
                default:
                    break;
            }
          
            
        }
    }

    private void OnBeforeModuleStateChange(BeforeModuleStateChangedEvent e)
    {
        Debug.Log($"[GameManager] {timeStamp.ToString()} Before state change of {e.SourceModule.GetType().Name} to {e.NewState}. Message: {e.Message}");
    }

    private void OnAfterModuleStateChange(AfterModuleStateChangedEvent e)
    {
        Debug.Log($"[GameManager] {timeStamp.ToString()} After state change of {e.SourceModule.GetType().Name} to {e.NewState}. Message: {e.Message}");
    }




}

