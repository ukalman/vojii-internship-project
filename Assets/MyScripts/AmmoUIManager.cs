using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUIManager : MonoBehaviour
{
    public GameObject bulletIconPrefab; // Assign this in the inspector with your bullet icon
    public PlayerAttack playerAttack; // Assign this in the inspector

    [SerializeField] private float fadeDuration = 1.0f; // Duration for the fade effect
    private bool isReloading;
    private bool noAmmoDisplay;
    private int _currentPistolAmmo = 0;
    private int _currentTotalAmmo = 0;
    private GameObject[] bulletIcons;

    [SerializeField] private Text ammoText;
    [SerializeField] private Text reloadingText; // Assign this in the inspector to a Text component

    void Start()
    {
        //currentAmmo = playerAttack.pistolAmmo; // Assuming starting ammo is always 15
        _currentPistolAmmo = playerAttack.pistolAmmo;
        _currentTotalAmmo = playerAttack.totalAmmo;
        bulletIcons = new GameObject[_currentPistolAmmo];

        // Instantiate bullet icons
        for (int i = 0; i < _currentPistolAmmo; i++)
        {
            bulletIcons[i] = Instantiate(bulletIconPrefab, transform);
            bulletIcons[i].transform.localScale = Vector3.one; // Ensure the icon has the correct scale
        }
        Debug.Log("first child: " + transform.GetChild(0).name);

        //ammoText = transform.parent.GetChild(0).GetComponent<Text>();
        reloadingText.color = new Color(reloadingText.color.r, reloadingText.color.g, reloadingText.color.b, 0); // Set initial alpha to 0
        UpdateAmmoText(_currentPistolAmmo, playerAttack.totalAmmo);
    }

    void Update()
    {
        // Update UI if ammo count changes
        if (_currentPistolAmmo != playerAttack.pistolAmmo || _currentTotalAmmo != playerAttack.totalAmmo)
        {
            UpdateAmmoDisplay(playerAttack.pistolAmmo, playerAttack.totalAmmo);
        }
        
        CheckReload();
       
    }

    void CheckReload()
    {
        // Check if the weapon is reloading or if there's no ammo left
        bool shouldDisplayNoAmmo = _currentPistolAmmo == 0 && _currentTotalAmmo == 0;
       

        if (isReloading != playerAttack.weaponReloading || shouldDisplayNoAmmo)
        {
            isReloading = playerAttack.weaponReloading;
            if (isReloading || shouldDisplayNoAmmo)
            {
                // Start the fade in/out coroutine with a conditional message
                if(!noAmmoDisplay) StartCoroutine(ReloadingFade(shouldDisplayNoAmmo ? "No Ammo!" : "Reloading...", shouldDisplayNoAmmo));
                
            }
            else
            {
                // Stop the coroutine and set alpha to 0 immediately
                StopAllCoroutines(); // Use StopAllCoroutines to ensure all instances of the coroutine are stopped
                reloadingText.color = new Color(reloadingText.color.r, reloadingText.color.g, reloadingText.color.b, 0);
                _currentTotalAmmo = playerAttack.totalAmmo;
            }
        }
    }

    void UpdateAmmoDisplay(int newAmmo, int totalAmmo)
    {
        // Enable/disable bullet icons based on current ammo
        for (int i = 0; i < bulletIcons.Length; i++)
        {
            bulletIcons[i].SetActive(i < newAmmo);
        }

        UpdateAmmoText(newAmmo, totalAmmo);

        _currentPistolAmmo = newAmmo;
    }

    void UpdateAmmoText(int newAmmo, int totalAmmo)
    {
        ammoText.text = "Ammo: " + newAmmo.ToString() + "/" + totalAmmo.ToString();

    }
    
    IEnumerator ReloadingFade(string displayText, bool shouldDisplayNoAmmo)
    {
        if (shouldDisplayNoAmmo) noAmmoDisplay = true;
        
        reloadingText.text = displayText;
        reloadingText.color = Color.red; // Set text color to red for both messages
        

        // Fade in
        while (reloadingText.color.a < 1)
        {
            reloadingText.color = new Color(reloadingText.color.r, reloadingText.color.g, reloadingText.color.b, reloadingText.color.a + Time.deltaTime);
            yield return null;
        }

        // Fade out
        while (reloadingText.color.a > 0)
        {
            reloadingText.color = new Color(reloadingText.color.r, reloadingText.color.g, reloadingText.color.b, reloadingText.color.a - Time.deltaTime);
            yield return null;
        }

        // Only repeat if still reloading or out of ammo
        if(isReloading || (_currentPistolAmmo == 0 && _currentTotalAmmo == 0))
        {
            //yield return new WaitForSeconds(0.5f); // Wait for a moment before restarting the fade effect
            StartCoroutine(ReloadingFade(displayText, shouldDisplayNoAmmo));
        }
    }
       
    
}
