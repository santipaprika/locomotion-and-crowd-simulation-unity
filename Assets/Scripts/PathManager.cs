using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [HideInInspector]
    public Collider plane;

    [HideInInspector]
    public Vector2 goal;


    // Start is called before the first frame update
    void Start()
    {
        // assign same plane object as in Crowd Generator object
        plane = GameObject.FindObjectOfType<CrowdGenerator>().plane;

        Vector3 minBound = plane.bounds.min;
        Vector3 maxBound = plane.bounds.max;
        goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), goal) < 0.1f)
        {
            Vector3 minBound = plane.bounds.min;
            Vector3 maxBound = plane.bounds.max;
            goal = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.z, maxBound.z));
        }
    }
}
