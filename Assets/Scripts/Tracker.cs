using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private Vector3 prevPosition;
    private Vector3 displacement;
    private Vector3 speed;
    private Vector3 localSpeed;
    private Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        displacement = gameObject.transform.position - prevPosition;
        speed = displacement / Time.deltaTime;
        localSpeed = transform.worldToLocalMatrix.MultiplyVector(speed);
        
        if (Mathf.Abs(speed.magnitude) > 0.00001f)
            forward = speed.normalized;

        prevPosition = gameObject.transform.position;
    }

    public float getSpeedX() {
        return localSpeed.x;
    }

    
    public float getSpeedZ() {
        return localSpeed.z;
    }

    public Vector2 getSpeedXZ() {
        return new Vector2(localSpeed.x, localSpeed.z);
    }

    public Vector3 getForward() {
        return forward;
    }

    void OnDrawGizmos() {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + speed / 10f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + forward * speed.magnitude / 5f);
    }
}
