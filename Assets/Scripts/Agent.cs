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
    void Update()
    {
        transform.position += velocity * Time.fixedDeltaTime;
        GetComponent<Rigidbody>().position += velocity * Time.fixedDeltaTime;
    }
}
