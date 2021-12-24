using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    private Animator _animator;
    private Tracker _tracker;
    private Vector2 _prevSpeedXZ;
    private Vector2 _currSpeedXZ;
    public float _interpolationSpeedFactor = 0.05f;
    public float _interpolationOrientationFactor = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _tracker = gameObject.GetComponent<Tracker>();
        _prevSpeedXZ = _tracker.getSpeedXZ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currSpeedXZ = Vector2.Lerp(_currSpeedXZ, _tracker.getSpeedXZ(), _interpolationSpeedFactor);
        _currSpeedXZ = _tracker.getSpeedXZ();
        _animator.SetFloat("Speed X", _currSpeedXZ.x);   
        // range between 'idle' and 'walk' interpolation is not correct. try to correct it.
        _animator.SetFloat("Speed Z", _currSpeedXZ.y > 1.591931f ? _currSpeedXZ.y : _currSpeedXZ.y * 1.5f);
        transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(transform.eulerAngles.y, _tracker.getEulerY(), _interpolationOrientationFactor), 0f);
    }
}
