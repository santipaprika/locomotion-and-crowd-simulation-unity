using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private float speedX = 0;
    private float speedZ = 0;
    private Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 speed = (gameObject.transform.position - prevPosition) / Time.deltaTime;
        speedX = speed.x;
        speedZ = speed.z;
        prevPosition = gameObject.transform.position;
    }

    public float getSpeedX() {
        return speedX;
    }

    
    public float getSpeedZ() {
        return speedZ;
    }
}
