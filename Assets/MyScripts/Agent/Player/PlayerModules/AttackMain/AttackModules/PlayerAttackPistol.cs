using System.Collections;
using UnityEngine;


public class PlayerAttackPistol : IPlayerAttack
{
    private bool isShot = false;

    private PlayerAttack _owner;


    public void Initialize(Rigidbody playerRigidbody, Transform playerTransform, PlayerAttack owner)
    {
        this._owner = owner;
        owner.pistolCrosshair.SetActive(true);
        owner.pistolDamage = 10f;
        owner.pistolRange = 100f;
        owner.pistolImpactForce = 80f;
        owner.pistolAmmo = 15;
        owner.totalAmmo = 15;

        isShot = false;
        owner.pistolCrosshair.SetActive(false);

    }


    public void Tick()
    {
        CheckShoot();
    }

    public void FixedTick()
    {

    }


    private void CheckShoot()
    {
        if (Input.GetButtonDown("Fire1") && !_owner.weaponReloading) // Fire1 is usually the left mouse button
        {

            if (!isShot && _owner.pistolAmmo > 0)
            {
                isShot = true;
                _owner.pistolAmmo--;
                _owner.StartCoroutine(SetIsShot());
                Debug.Log("Player shoots.");
                _owner.StartCoroutine(FireGun());
            }

            else if (_owner.pistolAmmo <= 0)
            {
                _owner.emptyGunSound.Play();

            }

        }
    }


    private IEnumerator SetIsShot()
    {
        yield return new WaitForSeconds(.4f);
        isShot = false;
    }

    public void Equip()
    {
        _owner.pistolCrosshair.SetActive(true);
        _owner.pistolAnimator.Play("PistolEquip",-1, 0f);
        _owner.gunEquipSound.Play();
    }

    public IEnumerator UnEquip()
    {
        _owner.pistolCrosshair.SetActive(false);
        _owner.pistolAnimator.Play("PistolUnEquip", -1, 0f);
        _owner.gunHolsterSound.Play();
        yield return new WaitForSeconds(.5f);
        //yield return new WaitForSeconds(2f);
    }

    private IEnumerator FireGun()
    {

        _owner.muzzleFlash.Play(); // Play the muzzle flash particle system
        _owner.gunshotSound.Play();

        GameObject impactGameObject = null;

        RaycastHit hit;
        if (Physics.Raycast(_owner.fpsCam.position, _owner.fpsCam.forward, out hit, _owner.pistolRange))
        {
            Debug.Log("Hit object: " + hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(_owner.pistolDamage);
            }

            
            if (hit.rigidbody != null)
            {
                Debug.Log("Yeah, I do have a rigidbody.");
                hit.rigidbody.AddForce(-hit.normal * _owner.pistolImpactForce);
            }
            

            // LookRotation and surface normal
            
            impactGameObject = Object.Instantiate(_owner.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));


        }

        yield return new WaitForSeconds(.1f);
        if (!_owner.weaponAimed)
        {
            _owner.pistolAnimator.Play("PistolRecoil", -1, 0f);
        } else _owner.pistolAnimator.Play("PistolAimRecoil", -1, 0f);

        _owner.pistolAnimator.Update(0f); // Forces the animator to immediately update to the new state

        if (impactGameObject != null)
        {
            Object.Destroy(impactGameObject, 1f);
        }

        if (hit.transform && hit.transform.name.Equals("AsynchronousButton"))
        {
            ActivateButtonShotEvent.BroadcastActivateButtonShot("AsynchronousButton");
        }
        
        if (hit.transform && hit.transform.name.Equals("SynchronousButton"))
        {
            ActivateButtonShotEvent.BroadcastActivateButtonShot("SynchronousButton");
        }
        
        if (hit.transform && hit.transform.name.Equals("ResetButton"))
        {
            Debug.Log("Reset button shot");
            ActivateButtonShotEvent.BroadcastActivateButtonShot("ResetButton");
        }
        
        if (hit.transform && hit.transform.name.Equals("Eye"))
        {
            Debug.Log("EYE SHOT");
            SauronEyeShotEvent.BroadcastSauronEyeShot(hit.transform);
        }

    }

    public IEnumerator ReloadWeapon()
    {
        if (_owner.weaponAimed)
        {
            
            _owner.pistolAnimator.SetBool("ReloadWhileAimed", true);
            Debug.Log("ReloadWhileAimed is true.");
            //yield return new WaitForSeconds(3f);
            Debug.Log("Reload anim ended..");
            // Wait until the "PistolReload" animation starts
            // yield return new WaitUntil(() => owner.pistolAnimator.GetCurrentAnimatorStateInfo(0).IsName("PistolReload"));
            // Debug.Log("PistolReload animation started!");
            // // Now wait until the "PistolReload" animation is no longer playing
            // yield return new WaitWhile(() => owner.pistolAnimator.GetCurrentAnimatorStateInfo(0).IsName("PistolReload"));
            // Debug.Log("PistolReload animation finished!");
            //owner.weaponAimed = false;
        } else
        {
            Debug.Log("ELSE CONDITION!!");
            _owner.pistolAnimator.Play("PistolReload", -1, 0f);
            _owner.pistolAnimator.Update(0f);
        } 
        yield return new WaitForSeconds(3f);


        if (_owner.pistolAmmo + _owner.totalAmmo >= 15)
        {
            
            _owner.totalAmmo -= (15 - _owner.pistolAmmo);
            _owner.pistolAmmo = 15;
        }
        
        
        else
        {
            
            _owner.pistolAmmo += _owner.totalAmmo;
            _owner.totalAmmo = 0;
        }
        _owner.pistolAnimator.SetBool("ReloadWhileAimed", false);
        Debug.Log("RELOADING FINISHED, CURRENT PISTOL AMMO: " + _owner.pistolAmmo);
        Debug.Log("RELOADING FINISHED, CURRENT TOTAL AMMO: " + _owner.totalAmmo);
    }


    public void HandleAim()
    {
        _owner.pistolAnimator.Play("PistolAim",-1,0f);
        _owner.pistolAnimator.Update(0f);
        _owner.pistolCrosshair.SetActive(false);
        _owner.pistolAnimator.SetBool("Unaimed", false);
        

    }
    
    public void HandleUnAim()
    {
        _owner.pistolCrosshair.SetActive(true);
        _owner.pistolAnimator.SetBool("Unaimed", true);
        _owner.weaponAimed = false;

    }
}
