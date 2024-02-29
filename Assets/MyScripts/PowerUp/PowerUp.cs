using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : ModuleBase
{
    public string powerupType;
    public static event Action<PowerUp> PowerUpPickedUpAction;


    public ParticleSystem PickupEffect;
    public GameObject cubeVisual;

    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 startScale;


    public AudioSource IdleSound;
    public AudioSource PickupSound;

    private bool pickedup;

    public float rotationSpeed = 50f; // Degrees per second
    public float hoverHeight = 0.5f; // Max height variation
    public float hoverFrequency = 2f; // Oscillations per second

    private float originalY;

    public override IEnumerator IE_Initialize()
    {
        yield return StartCoroutine(base.IE_Initialize());
        Debug.Log("My initialize is called!!");
        // Store the original Y position
        originalY = transform.position.y;
        pickedup = false;
    }

    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());
        //Debug.Log("Now my activate is called!");
        //Debug.Log("transform.position: " + transform.position);
        //Debug.Log("transform.rotation: " + transform.rotation);
        //Debug.Log("transform.localScale: " + transform.localScale);
        
        //transform.position = startPosition;
        //transform.rotation = startRotation;
        //transform.localScale = startScale;
       

        cubeVisual.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        if (IdleSound != null) {} //IdleSound.Play();

    }

    /*
    public override IEnumerator IE_Deactivate()
    {
        yield return StartCoroutine(base.IE_Deactivate());
        cubeVisual.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        if (IdleSound != null) IdleSound.Stop();
    }
    */

    public override void Deactivate()
    {
        cubeVisual.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        if (IdleSound != null) IdleSound.Stop();
    }


    public override bool Tick()
    {
        if (base.Tick())
        {
            if (!pickedup)
            {
                Debug.Log("My tick is working...");
            
                // Rotate
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

                var position = transform.position;
                // Hover
                var newY = originalY + Mathf.Sin(Time.time * hoverFrequency) * hoverHeight;
                transform.position = new Vector3(position.x, newY, position.z);
            }

            return true;
        }

        return false;

        



      
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (State != ModuleState.Activated) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup());
        }
    }

    IEnumerator Pickup()
    {
        Debug.Log("PowerUp picked up!");
        pickedup = true;
        // Pick Up Effect
        PickupSound.Play();
        ParticleSystem pickupEffectObject = Instantiate(PickupEffect, transform.position, transform.rotation);


        /*
        GetComponent<BoxCollider>().enabled = false;
        cubeVisual.GetComponent<MeshRenderer>().enabled = false;
        cubeVisual.GetComponent<BoxCollider>().enabled = false;
        */

        //IE_Deactivate();
        Deactivate();


        // Apply effect to the player
        GameManager.Instance.ActivatePowerUp(this.powerupType);
        

        yield return new WaitWhile(() => pickupEffectObject.isPlaying);

        Destroy(pickupEffectObject.gameObject);
        // Remove the power-up object
        PowerUpPickedUpEvent.BroadcastPowerUpPickedUp(this);
        //PowerUpPickedUpAction?.Invoke(this);
        Destroy(gameObject);
    }
}
