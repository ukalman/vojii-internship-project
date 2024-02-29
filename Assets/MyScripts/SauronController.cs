using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauronController : MonoBehaviour
{

    private bool _eyeShot;
    private bool _hasMoved;
    private bool _sauronEmerged;
    
    
    [SerializeField] private AudioSource _warHorn; 
    [SerializeField] private AudioSource _explosion;
    [SerializeField] private AudioSource _rumbling;
    [SerializeField] private AudioSource _sauronSpeech;
    [SerializeField] private AudioSource _flames;
    private void OnEnable()
    {
        SauronEyeShotEvent.OnSauronEyeShot += HandleSauronEyeShot;
    }

    private void OnDisable()
    {
        SauronEyeShotEvent.OnSauronEyeShot -= HandleSauronEyeShot;
    }

    // Start is called before the first frame update
    void Start()
    {
        _eyeShot = false;
        _hasMoved = false;
        _sauronEmerged = false;
        _flames.Play();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SauronEmerges());
    }

    void HandleSauronEyeShot(Transform hitTransform)
    {
        hitTransform.Find("Flames").gameObject.SetActive(true);
        _sauronSpeech.Play();
        _eyeShot = true;
    }

    IEnumerator SauronEmerges()
    {
        if (!_sauronEmerged && PlateController.hasMoved && _eyeShot)
        {
            _sauronEmerged = true;
            _warHorn.Play();
            yield return new WaitUntil(() => !_warHorn.isPlaying);
            transform.Find("SauronVisuals").gameObject.SetActive(true);
            _explosion.Play();
            _rumbling.Play();
        }
        
    }
}
