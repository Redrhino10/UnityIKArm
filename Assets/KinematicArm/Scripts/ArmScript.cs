using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    
    private GameObject ArmBase;
    private GameObject ArmEnd;
    private Vector3 PreviousPosition;

    private Vector3 Base = Vector3.zero;
    public GameObject BaseObject;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Base = new Vector3(0,0,0);
        if(BaseObject != null)
        {
            Base = BaseObject.transform.position;
        }

        Debug.Log(string.Format("Base is {0} for object {1}", 
            Base, gameObject)); 

        transform.position = Base;  //This is necessary it seems...
        Debug.Log(string.Format("obj: {0}, tp: {1}, base: {2}",
            gameObject.name, transform.position, Base));

        foreach(Transform x in gameObject.transform)
        {
            if(x.name == "ArmBase"|| x.name == "ArmBase1") { ArmBase = x.gameObject; }
            else if(x.name == "ArmEnd" || x.name == "ArmEnd1") { ArmEnd = x.gameObject; }
        }
        //ArmBase = gameObject.transform.Find("ArmBase").gameObject;
        //ArmEnd = gameObject.transform.Find("ArmEnd").gameObject;

        //transform.position = Base - ArmBase.transform.position;
        //transform.position -= ArmBase.transform.position;
        transform.position += Base - ArmBase.transform.position;


        Debug.Log(gameObject.name + "'s Base = " + ArmBase.ToString());
        //Debug.Log(string.Format("obj: {0}, tp: {1}, base: {2}",
        //    gameObject.name, transform.position, Base));
    }

    // Update is called once per frame
    void Update()
    {
        //Update base position
        if(transform.position != Base)
        {
            transform.position += Base - ArmBase.transform.position;
            Debug.Log("XXX");
        }

        timer += Time.deltaTime;
        if(timer >= 0.1f)
        {
            RotateTheArm();
            timer -= 0.1f;
        }
    }

    void RotateTheArm()
    {
        Debug.Log(string.Format("Previous Position = {0}, Current = {1}, End = {2}",
            PreviousPosition, ArmBase.transform.position, ArmEnd.transform.position));

        PreviousPosition = ArmBase.transform.position;
        transform.rotation *= Quaternion.Euler(-5f, -2f, -6f);
        transform.position -= ArmBase.transform.position - PreviousPosition;
        
    }
}
