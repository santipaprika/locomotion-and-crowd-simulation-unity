using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    private Animator animator;
    private Tracker tracker;
    private float prevSpeedX;
    private float prevSpeedZ;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        tracker = gameObject.GetComponent<Tracker>();
        prevSpeedX = tracker.getSpeedX();
        prevSpeedZ = tracker.getSpeedZ();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed X", (prevSpeedX + tracker.getSpeedX()) / 2.0f);   
        animator.SetFloat("Speed Z", (prevSpeedZ + tracker.getSpeedZ()) / 2.0f);   
    }
}
