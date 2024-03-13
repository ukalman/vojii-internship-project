using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : AgentControlBase
{

    private bool collidedWithBombCollectible = false;
    private bool collidedWithKatanaCollectible = false;
    private bool collidedWithPistolCollectible = false;

    /*
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
    */

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


    
    private void OnTriggerEnter(Collider other)
    {
        PlayerAttack PlayerAttackModule = GetModule<PlayerAttack>();
        Debug.Log("On Trigger enter, " + other.tag);
        
        if (other.CompareTag("PistolCollectible"))
        {
            Destroy(other.transform.parent.parent.gameObject);
            PlayerAttackModule.CollectPistol();
            
        } else if (other.CompareTag("BombCollectible") && PlayerAttackModule.bombCount < 5)
        {
            Destroy(other.transform.parent.parent.gameObject);
            PlayerAttackModule.CollectBomb();
            
        } else if (other.CompareTag("KatanaCollectible"))
        {
            Destroy(other.transform.parent.parent.parent.gameObject);
            PlayerAttackModule.CollectKatana();
            
        }
    }

    /*
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