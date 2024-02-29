using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    [SerializeField] private List<HeroPlate> _heroPlates;

    private bool isMoving;
    public static bool hasMoved;

    private void OnEnable()
    {
        ActivateButtonShotEvent.OnActivateButtonShot += HandleActivateButtonShot;
    }

    private void OnDisable()
    {
        ActivateButtonShotEvent.OnActivateButtonShot -= HandleActivateButtonShot;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(MoveThePlatesSynchronously());
        isMoving = false;
        hasMoved = false;
        //StartCoroutine(MoveThePlatesAsynchronously());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator MoveThePlatesAsynchronously()
    {
        foreach (var heroPlate in _heroPlates)
        {
            yield return StartCoroutine(heroPlate.MoveUpwards());
        }

        hasMoved = true;
        isMoving = false;
    }
    
    IEnumerator MoveThePlatesSynchronously()
    {
        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (var heroPlate in _heroPlates)
        {
            Coroutine coroutine = StartCoroutine(heroPlate.MoveUpwards());
            coroutines.Add(coroutine);
        }

        // Wait for all coroutines to finish
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        isMoving = false;
        hasMoved = true;
    }

    IEnumerator ResetThePlates()
    {
        Debug.Log(("RESET PLATES ARE CALLED!!"));
        hasMoved = false;

        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (var heroPlate in _heroPlates)
        {
            Coroutine coroutine = StartCoroutine(heroPlate.GoBackToOriginalPosition());
            coroutines.Add(coroutine);

        } 
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        isMoving = false;

    }

    void HandleActivateButtonShot(string name)
    {
        Debug.Log($"Handling Activate Button Shot for: {name}");
        switch (name)
        {
            case "AsynchronousButton":
                if (!isMoving && !hasMoved)
                {
                    isMoving = true;
                    StartCoroutine(MoveThePlatesAsynchronously());
                }
                
                break;
            case "SynchronousButton":
                if (!isMoving && !hasMoved)
                {
                    isMoving = true;
                    StartCoroutine(MoveThePlatesSynchronously());
                }
                
                break;
            case "ResetButton":
                StartCoroutine(ResetThePlates());
                break;
            default:
                break;
        }
        
        
    }
    
}
