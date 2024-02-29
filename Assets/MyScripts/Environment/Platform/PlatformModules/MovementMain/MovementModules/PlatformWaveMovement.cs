using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWaveMovement : MonoBehaviour
{

    public Vector3 movementAxis; // Editör'de ver, right için x = 1 diðerleri 0 unutma
    public float frequency; // Movement speed broski, editörde ver(1f iyi default)
    public float amplitude; // Movement amplitude broski, editörde ver(12f iyi default, right için)
    private Vector3 startPosition;

    public void Initialize()
    {
        startPosition = transform.position;

    }

    public void Tick()
    {
        Move();
    }

    public void FixedTick()
    {

    }

    private void Move()
    {
        Vector3 newPosition = startPosition + movementAxis * Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = newPosition;
    }

    void OnCollisionEnter(Collision collision)
    {

        collision.transform.SetParent(transform);

    }

    void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);

      
    }

}
