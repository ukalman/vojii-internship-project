using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAttack
{
    void Initialize(Transform playerTransform, PlayerAttack owner);
    void Tick();
    void FixedTick();

    IEnumerator Equip();

    IEnumerator UnEquip();

    void HandleAim();

    void HandleUnAim();

    IEnumerator ReloadWeapon();
}
