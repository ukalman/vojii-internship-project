using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBomb : IPlayerAttack
{
     
    private Transform playerTransform;
    private PlayerAttack _owner;

    public float BombThrowForce;

    private GameObject bombPrefab;
    private List<Rigidbody> BombRigidBodies;
    //private List<Bomb> Bombs;

    private List<Bomb> CopyBombs;


    private bool explodeBombsRandomly;
    private bool isRandomlyExploding;

    private bool isReadyToThrow;
    

    public void Initialize(Transform playerTransform, PlayerAttack owner)
    {
        Debug.Log("Player Attack Bomb's Initialize!");
        this.playerTransform = playerTransform;
        this._owner = owner;

        BombThrowForce = 5f;
        BombRigidBodies = new List<Rigidbody>();
        _owner.PlantedBombs = new List<Bomb>();

        bombPrefab = owner.bombPrefab;
        explodeBombsRandomly = false;
        isRandomlyExploding = false;

        isReadyToThrow = false;
    }

    public void Tick()
    {
        
        CheckBombInstantiation();

        foreach (Bomb bomb in _owner.PlantedBombs)
        {
            if (bomb != null)
            {
                bomb.Tick();
            }
        }

        CheckRandomExplosion(); // checks random explosion


    }

    public void FixedTick()
    {
        ThrowBombs();
        
        if (explodeBombsRandomly)
        {
            ExplodeBombsRandomly();
        }
        else if (isRandomlyExploding)
        {
            _owner.StartCoroutine(ExplodeBombsSequence());
        }
        else
        {
            _owner.StartCoroutine(ExplodeBombs());
        }



    }


    public IEnumerator Equip()
    {
        _owner.bombAnimator.Play("BombEquip",-1, 0f);
        yield return new WaitForSeconds(.5f);
        isReadyToThrow = true;
    }

    public IEnumerator UnEquip()
    {
        isReadyToThrow = false;

        
       
        _owner.bombAnimator.Play("BombUnEquip", -1, 0f);
        yield return new WaitForSeconds(.3f);
        
        
        
        yield return null;
    }
    

    void CheckBombInstantiation()
    {
        if (Input.GetMouseButtonDown(2) && _owner.bombCount > 0)
        {
            Debug.Log("Bomb instantiation initiates");
            _owner.StartCoroutine(InstantiateBomb());
        }
    }

    void CheckRandomExplosion()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            explodeBombsRandomly = true;
            isRandomlyExploding = true;
        }
    }
  

    IEnumerator ExplodeBombs()
    {
        CopyBombs = new List<Bomb>(_owner.PlantedBombs);
        for (int i = 0; i < _owner.PlantedBombs.Count; i++)
        {
            if (_owner.PlantedBombs[i] != null)
            {
                bool isExploded = _owner.PlantedBombs[i].FixedTick();
                if (isExploded)
                {

                    CopyBombs.Remove(_owner.PlantedBombs[i]);
                }
            }
        }

        // _owner.Bombs = new List<Bomb>(CopyBombs); Instead of doing this, do the thing below. Still referencing to same set of Bomb objects.

        _owner.PlantedBombs.Clear();
        _owner.PlantedBombs.AddRange(CopyBombs);

        if (_owner.PlantedBombs.Count <= 0 && _owner.bombCount <= 0)
        {
            yield return _owner.StartCoroutine(_owner.BombsRanOut());
        }

        yield return null;

    }






    private IEnumerator InstantiateBomb()
    {
        if (isReadyToThrow)
        {
            isReadyToThrow = false;
            Debug.Log("BOMB INSTANTIATED. " + Time.time);
            GameObject bomb = GameObject.Instantiate(bombPrefab, playerTransform.position + playerTransform.forward * 2f, playerTransform.rotation);
            Bomb bombScript = bomb.GetComponent<Bomb>();
            bombScript.Initialize();
            _owner.PlantedBombs.Add(bombScript);
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            BombRigidBodies.Add(rb);
            _owner.bombCount--;
        
            
            /*
            if (_owner.Bombs.Count <= 0)
            {
                _owner.StartCoroutine(UnEquip());
            }
            */

            if (_owner.bombCount > 0)
            {
                _owner.bombModel.SetActive(false);
                yield return new WaitForSeconds(.5f);
                _owner.bombModel.SetActive(true);
                _owner.bombAnimator.Play("BombEquip",-1, 0f);
                yield return new WaitForSeconds(.5f);
                isReadyToThrow = true;
            }
            else
            {
                _owner.bombModel.SetActive(false); 
            } 
        }
        
        

    }

    void ThrowBomb(Rigidbody rb)
    {
        rb.AddForce(playerTransform.forward * BombThrowForce, ForceMode.VelocityChange); // velocity change means we don't care about the mass of the rigidbody
    }

    void ThrowBombs()
    {
        
        
        foreach (Rigidbody rb in BombRigidBodies)
        {
  
            ThrowBomb(rb);
            PlayerAudio playerAudio = _owner.GetParent().GetModule<PlayerAudio>();
            if(playerAudio != null) playerAudio.PlayerAudioState = AudioState.ThrowBomb;
        

        } 
    
        BombRigidBodies.Clear();  
        
        
    }





    void ExplodeBombsRandomly()
    {
        if (explodeBombsRandomly && _owner.PlantedBombs.Count > 0)
        {
            explodeBombsRandomly = false;
            _owner.StartCoroutine(ExplodeBombsSequence());
            isRandomlyExploding = false;

        }
    }

    IEnumerator ExplodeBombsSequence()
    {
        CopyBombs = new List<Bomb>(_owner.PlantedBombs)
;
        foreach (Bomb bomb in _owner.PlantedBombs)
        {
            if (!bomb.isActivated)
            {
                CopyBombs.Remove(bomb);
            }
        }


        while (CopyBombs.Count > 0)
        {
            // Wait for a random amount of time between explosions
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));

            // Pick a random bomb and explode it
            int randomIndex = Random.Range(0, CopyBombs.Count);

            if (CopyBombs[randomIndex] != null)
            {
                CopyBombs[randomIndex].hasDetonated = true;
                CopyBombs[randomIndex].Explode();
                _owner.PlantedBombs.Remove(CopyBombs[randomIndex]);
                CopyBombs.RemoveAt(randomIndex);
               
            }
        }


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
