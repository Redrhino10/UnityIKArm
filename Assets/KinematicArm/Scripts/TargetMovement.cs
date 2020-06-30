using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{

    [Range(0.001f, 0.2f)]
    public float movespeed = 0.01f;
    [Header("Random Movement")]
    public bool bAutoMove = false;
    private Transform targetorigin;

    [Header("Target colour change")]
    public Material InContact;
    public Material NoContact;
    public GameObject ArmEnd;
    private float TargetRadius;

    void Start()
    {
        if (ArmEnd == null)
        {
            Debug.Log("No ArmEnd set on " + gameObject);
            return;
        }
        else
        {
            TargetRadius = gameObject.GetComponent<Transform>().lossyScale.x;
            Debug.Log("Radius = " + TargetRadius);
        }

        targetorigin = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
        ChangeMaterial();
        if(bAutoMove) { RandomMovement(); }
    }

    void RandomMovement()
    {
        
    }

    void ChangeMaterial()
    {
        if(Vector3.Distance(gameObject.transform.position, 
            ArmEnd.transform.position) < TargetRadius)
        {
            gameObject.GetComponent<Renderer>().material = InContact;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = NoContact;
        }
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
