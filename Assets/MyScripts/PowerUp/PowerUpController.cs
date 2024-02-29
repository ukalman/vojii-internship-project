using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : ModularObject
{
    public GameObject doubleJumpPrefab;
    public GameObject dashPrefab;
    public GameObject wallRunPrefab;

    private GameObject doubleJumpObject;
    private GameObject dashObject;
    private GameObject wallRunObject;

    private List<GameObject> powerUpObjects;
    
    private void Awake()
    {
        StartCoroutine(SetupPowerUps());
    
    }
    
    /*
    private void Start()
    {
        base.Awake();
    }
    */

    IEnumerator IE_Initialize()
    {
        
        doubleJumpObject = Instantiate(doubleJumpPrefab, transform);
        powerUpObjects.Add(doubleJumpObject);
        dashObject = Instantiate(dashPrefab, transform);
        powerUpObjects.Add(dashObject);
        wallRunObject = Instantiate(wallRunPrefab, transform);
        powerUpObjects.Add(wallRunObject);
        
        foreach (var powerUpObject in powerUpObjects)
        {
           
            _modules.Add(powerUpObject.GetComponent<PowerUp>());
        }
        
        yield return null;
        
    } 

    IEnumerator IE_DeInitialize()
    {
        Destroy(doubleJumpObject);
        Destroy(dashObject);
        Destroy(wallRunObject);
        powerUpObjects.Clear();
        Modules.Clear();
        _modules.Clear();
        yield return null;
    }

    private void OnEnable()
    {
        //PowerUp.PowerUpPickedUpAction += HandlePowerUpPickedUp;
        PowerUpPickedUpEvent.OnPowerUpPickedUp += HandlePowerUpPickedUp;
    }

    private void OnDisable()
    {
        // PowerUp.PowerUpPickedUpAction -= HandlePowerUpPickedUp;
        PowerUpPickedUpEvent.OnPowerUpPickedUp -= HandlePowerUpPickedUp;
    }

    
    private IEnumerator SetupPowerUps()
    {
        powerUpObjects = new List<GameObject>();
        yield return StartCoroutine(IE_Initialize());
       
        yield return StartCoroutine(base.SetupModules());

    }
    

    private void HandlePowerUpPickedUp(PowerUp pickedUpPowerUp)
    {
        for (int i = _modules.Count - 1; i >= 0; i--)
        {
            if (_modules[i] == pickedUpPowerUp)
            {
                _modules.RemoveAt(i);
                break;
            }
        }
    }

    public override IEnumerator IE_ResetAllModules()
    {
        yield return StartCoroutine(IE_DeInitialize());
        yield return StartCoroutine(IE_Initialize());
        Debug.Log("Power Up Modules Reset.");
    }


}
