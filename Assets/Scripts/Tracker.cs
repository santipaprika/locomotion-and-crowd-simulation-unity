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
    private float eulerY;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        displacement = gameObject.transform.position - prevPosition;
        speed = displacement / Time.fixedDeltaTime;
        localSpeed = transform.worldToLocalMatrix.MultiplyVector(speed);

        if (Mathf.Abs(speed.magnitude) > 0.01f) {
            forward = speed.normalized;
            if (Mathf.Abs(speed.x) < 0.001f) {
                eulerY = speed.z > 0f ? 0f : Mathf.PI;
            } else {
                eulerY = Mathf.Atan(speed.x / speed.z);
            }
        }

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

    public float getEulerY() {
        return eulerY * Mathf.Rad2Deg;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + forward * speed.magnitude / 5f);
    }

}
