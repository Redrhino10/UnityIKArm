using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    
    private GameObject ArmBase;
    private GameObject ArmEnd;

    private Vector3 Base = Vector3.zero;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Base = new Vector3(0,0,0);

        transform.position = Base;  //This is necessary it seems...

        ArmBase = gameObject.transform.Find("ArmBase").gameObject;
        ArmEnd = gameObject.transform.Find("ArmEnd").gameObject;

        transform.position = Base - ArmBase.transform.position;

        Debug.Log("Base = " + ArmBase.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 0.1f)
        {
            RotateTheArm();
            timer -= 0.1f;
        }
    }

    void RotateTheArm()
    {
        transform.rotation *= Quaternion.Euler(-5f, -2f, -6f);
        transform.position -= ArmBase.transform.position;
    }
}
