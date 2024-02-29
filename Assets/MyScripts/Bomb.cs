using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float ActivationTime;
    public float Radius;
    public float ExplosionForce;

    public bool isActivated;
    public bool isTriggered;
    public bool hasDetonated;
    private bool isExploding;

    public ParticleSystem ExplosionEffect;
    public AudioSource bombActivatedSound;
    public AudioSource bombExplosionSound;

    private ParticleSystem explosionInstance;


    public void Initialize()
    {

        ActivationTime = 3f;
        Radius = 5f;
        ExplosionForce = 2000f;

        isActivated = false;
        isTriggered = false;
        hasDetonated = false;
        isExploding = false;

        StartCoroutine(SetActivated());
        explosionInstance = Instantiate(ExplosionEffect, transform.position, transform.rotation);

    }

    
    public void Tick()
    {
        CheckTrigger();
        Detonate();

        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateParticleColors();
        }

    }
    

    
    public bool FixedTick()
    {
        return Explode();
    }
    

    IEnumerator SetActivated()
    {
        yield return new WaitForSeconds(ActivationTime);
        isActivated = true;
        bombActivatedSound.Play();
    }

    
    public void CheckTrigger()
    {
        if ((Input.GetKeyDown(KeyCode.F)) && isActivated)
        {
            isTriggered = true;
        }

    }


    public void Detonate()
    {
        if(isActivated && isTriggered)
        {
            hasDetonated = true;
        }
    }


    public bool Explode()
    {
        if (hasDetonated && !isExploding)
        {
            
            bombExplosionSound.Play();
            isExploding = true;
            explosionInstance.Play();


            StartCoroutine(ExplodeTheBomb());
                                            
            // Wait for the explosion effect to finish then destroy the effect and this game object
            StartCoroutine(DestroyAfterEffect(explosionInstance));

            return true;
 
        }
        return false;
        
    }


    IEnumerator DestroyAfterEffect(ParticleSystem effectInstance)
    {
        yield return new WaitWhile(() => effectInstance.isPlaying);
        
        Destroy(effectInstance.gameObject);

        Destroy(gameObject);
    }

    IEnumerator ExplodeTheBomb()
    {
        yield return new WaitForSeconds(1f);

        GameObject originalGameObject = gameObject;
        Destroy(originalGameObject.transform.GetChild(0).transform.GetChild(0).gameObject);
        Destroy(originalGameObject.transform.GetChild(0).transform.GetChild(1).gameObject);

        // Get nearby objects
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, Radius);

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Rigidbody rb = nearbyCollider.GetComponent<Rigidbody>();
            if (rb != null && !rb.CompareTag("BombVisual"))
            {
                // an immaculate function
                rb.AddExplosionForce(ExplosionForce, transform.position, Radius);
            }
        }


        GetComponent<BoxCollider>().enabled = false;

    }

    private void UpdateParticleColors()
    {

        if (ExplosionEffect != null)
        {
            ParticleSystem.MainModule main = ExplosionEffect.main; 
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(Random.value, Random.value, Random.value));

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ExplosionEffect.colorOverLifetime;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(Random.value, Random.value, Random.value), 0.0f), new GradientColorKey(new Color(Random.value, Random.value, Random.value), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
        }
    }

 

}
