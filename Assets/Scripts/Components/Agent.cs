using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [HideInInspector]
    public PathManager pathManager = null;

    // [HideInInspector]
    public Vector3 velocity = new Vector3(0, 0, 0);
    public int id;
    static int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        id = counter++;
        pathManager = GetComponent<PathManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pathManager == null || pathManager.moveAgent)
        {
            GetComponent<Rigidbody>().position += velocity * Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (pathManager != null)
        {
            Gizmos.DrawLine(transform.position, new Vector3(pathManager.goal.x, 0, pathManager.goal.y));
        }
    }
}
