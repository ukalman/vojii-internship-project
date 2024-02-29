using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum AttackState
{
    Idle,
    ThrowBomb,
    Pistol,
    Katana
}
public class PlayerAttack : AgentModuleBase
{
    public AttackState attackState;


    // Bomb   
    private IPlayerAttack _bombModule;
    [Header("Bomb Properties")]
    public GameObject bombPrefab;
    public int bombCount = 3; // make this private later

    // Pistol (Glock)
    private IPlayerAttack _pistolModule;
    [Header("Pistol Properties")]
    public GameObject pistolModel;
    public GameObject pistolCrosshair;
    public Animator pistolAnimator;
    public ParticleSystem muzzleFlash;
    public AudioSource gunEquipSound;
    public AudioSource gunHolsterSound;
    public AudioSource gunshotSound;
    public AudioSource emptyGunSound;
    public AudioSource gunReloadSound;
    public GameObject impactEffect;
    public Transform fpsCam;
    public float pistolDamage = 10f;
    public float pistolRange = 100f;
    public float pistolImpactForce = 80f;
    public int totalAmmo = 15; 
    public int pistolAmmo = 15; // make this private later 
   
    // Katana
    private IPlayerAttack _katanaModule;
    [Header("Katana Properties")] 
    public GameObject katanaModel;
    public Animator katanaAnimator;
    public KatanaStaminaBar staminaBar;
    public AudioSource katanaDrawSound;
    public AudioSource katanaSheatheSound;
    public AudioSource katanaSwingSound1;
    public AudioSource katanaSwingSound2;
    public AudioSource katanaSwingSound3;
    public AudioSource katanaPowerAttackSound;
    public AudioSource katanaSlashSound;
    public float katanaDamage = 20f;
    public float katanaStamina = 100f;

    private bool _noWeaponEquipped = true;
    private bool _pistolEquipped = false;
    private bool _bombEquipped = false;
    private bool _katanaEquipped = false;

    public bool weaponAimed = false;
    public bool weaponReloading = false;

    public Rigidbody Rigidbody;

    private void OnEnable()
    {
        WeaponAimedEvent.OnWeaponAimed += HandleWeaponAim;
        WeaponUnAimedEvent.OnWeaponUnAimed += HandleWeaponUnAim;
    }

    private void OnDisable()
    {
        WeaponAimedEvent.OnWeaponAimed -= HandleWeaponAim;
        WeaponUnAimedEvent.OnWeaponUnAimed -= HandleWeaponUnAim;
    }

    public override IEnumerator IE_Initialize()
    {
        yield return StartCoroutine(base.IE_Initialize());

        attackState = AttackState.Idle;

        Rigidbody = GetComponent<Rigidbody>();

        _bombModule = new PlayerAttackBomb();
        _bombModule.Initialize(Rigidbody, transform, this);

        _pistolModule = new PlayerAttackPistol();
        _pistolModule.Initialize(Rigidbody, transform, this);

        pistolModel.SetActive(false);

        _katanaModule = new PlayerAttackKatana();
        _katanaModule.Initialize(Rigidbody, transform, this);
        
        katanaModel.SetActive(false);


    }

    public override bool Tick()
    {

        if (base.Tick())
        {
            CheckWeaponEquipment();

            CheckWeaponAim();

            StartCoroutine(CheckWeaponReload());

            _bombModule.Tick();

            if(_pistolEquipped) _pistolModule.Tick();
            
            else if(_katanaEquipped) _katanaModule.Tick();

            return true;
        }

        return false;
        
    }

    public override bool FixedTick()
    {
        if (base.FixedTick())
        { 
            _bombModule.FixedTick();
            if(_pistolEquipped) _pistolModule.FixedTick();
            
            else if(_katanaEquipped) _katanaModule.FixedTick();
            return true;
        }

        return false;
       

    }


    public void CheckWeaponEquipment()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(UnEquip());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
          
            if (!_pistolEquipped)
            {
                StartCoroutine(EquipPistol());
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
          
            if (!_katanaEquipped)
            {
                StartCoroutine(EquipKatana());
            }
        }

        
    }

    public void CheckWeaponAim()
    {
        if (Input.GetButtonDown("Fire2") && !weaponAimed && !weaponReloading)
        {
            weaponAimed = true;
            WeaponAimedEvent.BroadcastWeaponAim(attackState);
           
        } else if (Input.GetButtonDown("Fire2") && weaponAimed && !weaponReloading)
        {
            weaponAimed = false;
            WeaponUnAimedEvent.BroadcastWeaponUnAim(attackState);
          
   
        }
        
    }

    public IEnumerator CheckWeaponReload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (_pistolEquipped && pistolAmmo < 15 && !weaponReloading && totalAmmo > 0)
            {
                if(weaponAimed) WeaponUnAimedEvent.BroadcastWeaponUnAim(attackState);
                yield return new WaitForSeconds(.5f);
                gunReloadSound.Play();
                weaponReloading = true;
                yield return StartCoroutine(_pistolModule.ReloadWeapon());
                weaponReloading = false;
                
            }
        }

        yield return null;
    }

    public IEnumerator EquipPistol()
    {
        if (!_noWeaponEquipped) yield return StartCoroutine(UnEquip());
       
        attackState = AttackState.Pistol;
        pistolModel.SetActive(true);
        _pistolEquipped = true;
        _noWeaponEquipped = false;
        _pistolModule.Equip();
        WeaponEquippedEvent.BroadcastWeaponEquipment(attackState);
        yield return null;

    }

    public IEnumerator EquipKatana()
    {
        if (!_noWeaponEquipped) yield return StartCoroutine(UnEquip());
        
        attackState = AttackState.Katana;
        katanaModel.SetActive(true);
        _katanaEquipped = true;
        _noWeaponEquipped = false;
        _katanaModule.Equip();
        WeaponEquippedEvent.BroadcastWeaponEquipment(attackState);
        yield return null;
    }

    public IEnumerator UnEquip()
    {
        if (_pistolEquipped)
        {
            yield return StartCoroutine(_pistolModule.UnEquip());
            //yield return new WaitForSeconds(.5f);

            pistolModel.SetActive(false);
            _pistolEquipped = false;
            _noWeaponEquipped = true;
            //pistolModule.Equip();
            
            attackState = AttackState.Idle;
            WeaponEquippedEvent.BroadcastWeaponEquipment(attackState);
            yield return new WaitForSeconds(.4f);
        }
        
        else if (_katanaEquipped)
        {
            yield return StartCoroutine(_katanaModule.UnEquip());
            //yield return new WaitForSeconds(1f);
            
            katanaModel.SetActive(false);
            katanaModel.transform.localPosition = new Vector3(0.002990723f, -0.39324f, 0.8000031f);
            _katanaEquipped = false;
            _noWeaponEquipped = true;
            attackState = AttackState.Idle;
            WeaponEquippedEvent.BroadcastWeaponEquipment(attackState);
            yield return new WaitForSeconds(.2f);
        }
    }

    void HandleWeaponAim(AttackState attackState)
    {
        switch (attackState)
        {
            case AttackState.Idle:
                break;
            case AttackState.ThrowBomb:
                break;
            case AttackState.Pistol:
                _pistolModule.HandleAim();
                break;
            default:
                break;
        }

    }
    
    void HandleWeaponUnAim(AttackState attackState)
    {
        switch (attackState)
        {
            case AttackState.Idle:
                break;
            case AttackState.ThrowBomb:
                break;
            case AttackState.Pistol:
                _pistolModule.HandleUnAim();
                break;
            default:
                break;
        }

    }

    
}
