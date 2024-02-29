using System.Collections;
using UnityEngine;



public class PlayerAttackKatana : IPlayerAttack
{

    private PlayerAttack _owner;

    private bool _firstAttackMoveMade;
    private bool _secondAttackMoveMade;
    private bool _thirdAttackMoveMade;
    
    private bool _isRecoveringStamina = false;
    private float _staminaRecoveryRate = 15f; // Stamina points recovered per second
    private float _maxStamina = 100f; 
    
    
    public void Initialize(Rigidbody playerRigidbody, Transform playerTransform, PlayerAttack owner)
    {

        _owner = owner;
        _owner.katanaDamage = 20f;
        _owner.katanaStamina = 100f;
        _owner.staminaBar.SetMaxStamina(_owner.katanaStamina);
    }

    public void Tick()
    {
        _owner.StartCoroutine(CheckSwing());
        _owner.staminaBar.SetStamina(_owner.katanaStamina);

        CheckRecover();
    }

    public void FixedTick()
    {
        
    }

    public void Equip()
    {
        _owner.katanaAnimator.Play("KatanaEquip",-1, 0f);
        _owner.katanaDrawSound.Play();
    }

    public IEnumerator UnEquip()
    {
        _owner.katanaAnimator.Play("KatanaUnEquip", -1, 0f);
        yield return new WaitForSeconds(.5f);
        _owner.katanaSheatheSound.Play();
        yield return new WaitForSeconds(.4f);
    }

    private IEnumerator CheckSwing()
    {
        if (Input.GetButtonDown("Fire1") && _owner.katanaStamina >= 25)
        {
            yield return _owner.StartCoroutine(SwingKatana());
        }
    }

    private void CheckRecover()
    {
        if (_owner.katanaAnimator.GetCurrentAnimatorStateInfo(0).IsName("KatanaIdle") &&
            (_owner.katanaStamina < _maxStamina) && !_isRecoveringStamina)
        {
            _owner.StartCoroutine(RecoverStamina());
        }
    }
    
   
    
    private IEnumerator SwingKatana()
    {
        AnimatorStateInfo katanaStateInfo = _owner.katanaAnimator.GetCurrentAnimatorStateInfo(0);

        Debug.Log("Katana swung!");
        
        if (katanaStateInfo.IsName("KatanaIdle"))
        {
           
            _owner.katanaAnimator.Play("KatanaAttack1", -1, 0f);
            _owner.katanaAnimator.Update(0f);
            _owner.katanaSwingSound1.Play();
            _owner.katanaStamina -= 20f;


        } else if (katanaStateInfo.IsName("KatanaAttack1IntermediateKatanaAttack2"))
        {
            _owner.katanaAnimator.SetBool("secondSwing",true);
            _owner.katanaSwingSound2.Play();
            yield return new WaitWhile(() => katanaStateInfo.IsName("KatanaAttack2"));
            _owner.katanaStamina -= 25f;
            _owner.katanaAnimator.SetBool("secondSwing",false);
        } else if (katanaStateInfo.IsName("KatanaAttack2IntermediateKatanaAttack3"))
        {
            _owner.katanaAnimator.SetBool("thirdSwing",true);
            _owner.katanaSwingSound3.Play();
            yield return new WaitWhile(() => katanaStateInfo.IsName("KatanaAttack3"));
            _owner.katanaStamina -= 15f;
            _owner.katanaAnimator.SetBool("thirdSwing",false);
        }  else if (katanaStateInfo.IsName("KatanaAttack3IntermediateKatanaIdle"))
        {
            _owner.katanaSwingSound2.Play();
            _owner.katanaAnimator.Play("KatanaAttackRestarter", -1, 0f);
            _owner.katanaAnimator.Update(0f);
            _owner.katanaStamina -= 20f;

        } 
        
       
      
    }
    
    private IEnumerator RecoverStamina()
    {
        _isRecoveringStamina = true;

        while (_owner.katanaStamina < _maxStamina)
        {
            _owner.katanaStamina += _staminaRecoveryRate * Time.deltaTime; // Recover stamina over time
            _owner.katanaStamina = Mathf.Clamp(_owner.katanaStamina, 0, _maxStamina); // Ensure stamina does not exceed max

            yield return null; // Wait for the next frame
        }

        _isRecoveringStamina = false;
    }

    public void HandleAim()
    {
        
    }

    public void HandleUnAim()
    {
        
    }

    public IEnumerator ReloadWeapon()
    {
        yield return null;
    }
}
