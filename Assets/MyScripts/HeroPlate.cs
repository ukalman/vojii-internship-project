using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlate : MonoBehaviour
{
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveDistance;
    [SerializeField] private string plateName;
    [SerializeField] private AudioSource catchphraseSound;
    
    private Vector3 _originalPosition;

    private bool _isMoving;
    private bool _isTriggered;
    private bool _isMoved;
    void Start()
    {
        _originalPosition = transform.position;
        _isMoving = false;
        _isTriggered = false;
        _isMoved = false;
        
        //MoveUpwards(moveDistance);
    }
    
    public IEnumerator MoveUpwards()
    {
        Vector3 newPosition = new Vector3(transform.position.x, _originalPosition.y + moveDistance, transform.position.z);
        StopAllCoroutines(); //  ensure that no other coroutines are running on the same GameObject that might interfere with the current movement operation 
        yield return StartCoroutine(MoveOverTime(newPosition, moveDuration,false));
    }

    public IEnumerator GoBackToOriginalPosition()
    {
        StopAllCoroutines(); //  ensure that no other coroutines are running on the same GameObject that might interfere with the current movement operation 
        yield return StartCoroutine(MoveOverTime(_originalPosition, moveDuration,true));
    }

    IEnumerator MoveOverTime(Vector3 targetPosition, float duration, bool isResetting)
    {
        if(!isResetting) catchphraseSound.Play();
        
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            // Interpolate only the y component
            float newY = Mathf.Lerp(startPosition.y, targetPosition.y, time / duration);
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            time += Time.deltaTime;
            yield return null; // Wait till the next frame broski
        }

        // Ensure the object is exactly at the target position when done
        transform.position = new Vector3(startPosition.x, targetPosition.y, startPosition.z);
        if(!isResetting) yield return new WaitUntil(() => !catchphraseSound.isPlaying);

    }

  
    
}
