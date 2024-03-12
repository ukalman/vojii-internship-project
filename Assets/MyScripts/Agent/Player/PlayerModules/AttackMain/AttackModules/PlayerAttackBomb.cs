using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBomb : IPlayerAttack
{
     
    private Transform playerTransform;
    private PlayerAttack _owner;

    public float BombThrowForce;

    public GameObject bombPrefab;
    public List<Rigidbody> BombRigidBodies;
    public List<Bomb> Bombs;

    public List<Bomb> CopyBombs;


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
        Bombs = new List<Bomb>();

        bombPrefab = owner.bombPrefab;
        explodeBombsRandomly = false;
        isRandomlyExploding = false;

        isReadyToThrow = false;
    }

    public void Tick()
    {
        
        CheckBombInstantiation();

        foreach (Bomb bomb in Bombs)
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
            ExplodeBombs();
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

        if (_owner.bombCount <= 0)
        {
            _owner.StartCoroutine(_owner.UnEquip());
        }
        else
        {
            _owner.bombAnimator.Play("BombUnEquip", -1, 0f);
            yield return new WaitForSeconds(.3f);
        }
        
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
  

    void ExplodeBombs()
    {
        CopyBombs = new List<Bomb>(Bombs);
        for (int i = 0; i < Bombs.Count; i++)
        {
            if (Bombs[i] != null)
            {
                bool isExploded = Bombs[i].FixedTick();
                if (isExploded)
                {

                    CopyBombs.Remove(Bombs[i]);
                }
            }
        }

        Bombs = new List<Bomb>(CopyBombs);
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
            Bombs.Add(bombScript);
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            BombRigidBodies.Add(rb);
            _owner.bombCount--;
        

            if (Bombs.Count <= 0)
            {
                _owner.StartCoroutine(UnEquip());
            }

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
        if (explodeBombsRandomly && Bombs.Count > 0)
        {
            explodeBombsRandomly = false;
            _owner.StartCoroutine(ExplodeBombsSequence());
            isRandomlyExploding = false;

        }
    }

    IEnumerator ExplodeBombsSequence()
    {
        CopyBombs = new List<Bomb>(Bombs)
;
        foreach (Bomb bomb in Bombs)
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
                Bombs.Remove(CopyBombs[randomIndex]);
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
