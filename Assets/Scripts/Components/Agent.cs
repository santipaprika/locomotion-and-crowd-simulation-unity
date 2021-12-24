using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [HideInInspector]
    public PathManager pathManager;

    // [HideInInspector]
    public Vector3 velocity = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        pathManager = GetComponent<PathManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // transform.position += velocity * Time.deltaTime;
        GetComponent<Rigidbody>().position += velocity * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(pathManager.goal.x, 0, pathManager.goal.y));
    }
}
