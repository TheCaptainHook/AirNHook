using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BasicMoveTest : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var randPos = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        transform.position += randPos;
    }
}
