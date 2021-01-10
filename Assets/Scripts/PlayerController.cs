using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D _rigidbody;
    Transform _transform;
    Animator _animator;
    AudioSource _audio;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null) // if Rigidbody is missing
            Debug.LogError("Rigidbody2D component missing from this gameobject");

        _animator = GetComponent<Animator>();
        if (_animator == null) // if Animator is missing
            Debug.LogError("Animator component missing from this gameobject");
        _transform = GetComponent<Transform>();
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // let's just add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
