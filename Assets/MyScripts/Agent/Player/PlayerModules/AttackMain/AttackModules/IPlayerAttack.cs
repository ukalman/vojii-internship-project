using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAttack
{
    void Initialize(Rigidbody playerRigidbody, Transform playerTransform, PlayerAttack owner);
    void Tick();
    void FixedTick();

    void Equip();

    IEnumerator UnEquip();

    void HandleAim();

    void HandleUnAim();

    IEnumerator ReloadWeapon();
}
