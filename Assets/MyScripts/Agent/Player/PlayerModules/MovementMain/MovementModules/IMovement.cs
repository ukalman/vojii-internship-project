using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    //void Initialize(Rigidbody playerRigidbody, Transform playerTransform, MonoBehaviour owner);
    void Initialize(CharacterController playerController, Transform playerTransform, MonoBehaviour owner);
    void Tick();
    void FixedTick();
}
