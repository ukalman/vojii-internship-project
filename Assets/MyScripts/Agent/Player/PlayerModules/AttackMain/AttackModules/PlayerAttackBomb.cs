using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBomb : IPlayerAttack
{

    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private PlayerAttack owner;

    public float BombThrowForce;

    public GameObject bombPrefab;
    public List<Rigidbody> BombRigidBodies;
    public List<Bomb> Bombs;

    public List<Bomb> CopyBombs;


    private bool explodeBombsRandomly;
    private bool isRandomlyExploding;


    public void Initialize(Rigidbody playerRigidbody, Transform playerTransform, PlayerAttack owner)
    {
        this.playerRigidbody = playerRigidbody;
        this.playerTransform = playerTransform;
        this.owner = owner;

        BombThrowForce = 5f;
        BombRigidBodies = new List<Rigidbody>();
        Bombs = new List<Bomb>();

        bombPrefab = owner.bombPrefab;
        explodeBombsRandomly = false;
        isRandomlyExploding = false;
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
            owner.StartCoroutine(ExplodeBombsSequence());
        }
        else
        {
            ExplodeBombs();
        }



    }


    public void Equip()
    {

    }

    public IEnumerator UnEquip()
    {
        yield return null;
    }

 

    void CheckBombInstantiation()
    {
        if (Input.GetMouseButtonDown(2) && owner.bombCount > 0)
        {
            InstantiateBomb();
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






    private void InstantiateBomb()
    {
        GameObject bomb = GameObject.Instantiate(bombPrefab, playerTransform.position + playerTransform.forward * 2f, playerTransform.rotation);
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.Initialize();
        Bombs.Add(bombScript);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        BombRigidBodies.Add(rb);
        owner.bombCount--;

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
            PlayerAudio playerAudio = owner.GetParent().GetModule<PlayerAudio>();
            if(playerAudio != null) playerAudio.PlayerAudioState = AudioState.ThrowBomb;


        }

        BombRigidBodies.Clear();
    }





    void ExplodeBombsRandomly()
    {
        if (explodeBombsRandomly && Bombs.Count > 0)
        {
            explodeBombsRandomly = false;
            owner.StartCoroutine(ExplodeBombsSequence());
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
