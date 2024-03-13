using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TNTUIManager : MonoBehaviour
{
    
    public PlayerAttack playerAttack; 
    
    public GameObject tntIconPrefab;
    [SerializeField] private Text tntText; 
    
    private GameObject[] tntIcons;
    
    [SerializeField] private int _currentBombCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentBombCount = playerAttack.bombCount;
        
        tntIcons = new GameObject[5];

        // Maximum 5 TNTs
        for (int i = 0; i < 5; i++)
        {
            tntIcons[i] = Instantiate(tntIconPrefab, transform);
            tntIcons[i].transform.localScale = Vector3.one; // Ensure the icon has the correct scale
            tntIcons[i].SetActive(i < _currentBombCount);
        }
        
        UpdateTNTText();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentBombCount != playerAttack.bombCount)
        {
            UpdateTNTDisplay(playerAttack.bombCount);
            
        }
        
    }

    void UpdateTNTDisplay(int newBombCount)
    {
        _currentBombCount = newBombCount;
        
        for (int i = 0; i < tntIcons.Length; i++)
        {
            tntIcons[i].SetActive(i < _currentBombCount);
        }
        
        UpdateTNTText();
    }
    
    void UpdateTNTText()
    {
        tntText.text = "TNT: x" + _currentBombCount.ToString();
    }
    
}
