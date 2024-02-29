using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : AgentControlBase
{

    private void OnCollisionEnter(Collision collision)
    {
        if (Modules.Any(module => module is PlayerMovement))
        {
            var playerMovementModule = GetModule<PlayerMovement>();
            if (playerMovementModule != null)
            {
                playerMovementModule.HandleCollisionEnter(collision);
            }
        }
    }

    /*
    private void OnCollisionExit(Collision collision)
    {
        if (Modules.Any(module => module is PlayerMovement))
        {
            var playerMovementModule = GetModule<PlayerMovement>();
            if (playerMovementModule != null)
            {
                playerMovementModule.HandleCollisionExit(collision);
            }
        }
    }
    */


    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rope"))
        {
            Debug.Log("Trigger with Rope.");
            var playerMovementModule = GetModule<PlayerMovement>();
            if (playerMovementModule != null)
            {
                playerMovementModule.GetReadyToGrabRope(other);
            }
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger with Rope.");
        var playerMovementModule = GetModule<PlayerMovement>();
        if (playerMovementModule != null)
        {
            playerMovementModule.GetReadyToReleaseRope(other);
        }
    }
    */
}