using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    // Check Q,W,E,A,S,D keys for movement
    void CheckInputs()
    {
        if(Input.GetKey("w"))
        {
            gameObject.transform.position += Vector3.up * 0.01f;
        }
        if (Input.GetKey("s"))
        {
            gameObject.transform.position += Vector3.down * 0.01f;
        }
        if (Input.GetKey("a"))
        {
            gameObject.transform.position += Vector3.left * 0.01f;
        }
        if (Input.GetKey("d"))
        {
            gameObject.transform.position += Vector3.right * 0.01f;
        }
        if (Input.GetKey("q"))
        {
            gameObject.transform.position += Vector3.forward * 0.01f;
        }
        if (Input.GetKey("e"))
        {
            gameObject.transform.position += Vector3.back * 0.01f;
        }
    }

}
