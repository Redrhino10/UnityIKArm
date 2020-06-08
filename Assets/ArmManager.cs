using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmManager : MonoBehaviour
{
    [System.Serializable]
    public struct ArmInfo
    {
        public GameObject Arm;
        [HideInInspector]
        public GameObject ArmBase;  //The joint for the current arm
        [HideInInspector]
        public GameObject ArmEnd; //The socket for the next arm
        [HideInInspector]
        public Vector3 Socket; //The place it meets the previous arm/base
    };

    private float Timer = 0.0f;
    
    public ArmInfo[] ArmsList = new ArmInfo[0];
    public GameObject BaseObject;

    // Start is called before the first frame update
    void Start()
    {
        //debug for empty array
        if(ArmsList.Length == 0)
        {
            Debug.LogError(gameObject.name + "'s ArmList is empty?");
            return;
        }

        //set the base of the arm
        if (BaseObject == null)
        {
            Debug.LogWarning(gameObject.name +
                " has no BaseObject set; using itself as placeholder");
            BaseObject = gameObject;
        }

        //set default armbase and armend values for each arm
        for (int x = 0; x < ArmsList.Length; x++)
        {
            ArmsList[x].ArmBase = ArmsList[x].Arm.transform.Find("ArmBase").gameObject;
            ArmsList[x].ArmEnd = ArmsList[x].Arm.transform.Find("ArmEnd").gameObject;
            if (x == 0) { ArmsList[x].Socket = BaseObject.transform.position; }
            else
            {
                ArmsList[x].Socket = ArmsList[x-1].ArmEnd.transform.position;
            }
        }

        

        //set initial position for first arm attached to base
        ArmsList[0].Arm.transform.position += 
            BaseObject.transform.position - 
            ArmsList[0].ArmBase.transform.position;

        //set initial positions for other arms to be the end of previous arm
        //start array at one to be 2nd arm, then x-1 is previous arm
        for (int x = 1; x < ArmsList.Length; x++)
        {
            ArmsList[x].Arm.transform.position +=
            ArmsList[x-1].ArmEnd.transform.position -
            ArmsList[x].ArmBase.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= 0.1f)
        {
            RotateTheArm();
            Timer -= 0.1f;
        }
    }

    void RotateTheArm()
    {
        

        for (int x = 0; x < ArmsList.Length; x++)
        {
            ArmInfo ArmX = ArmsList[x];

            if(x == 0)
            {
                ArmX.Socket = BaseObject.transform.position;
            }
            else
            {
                ArmX.Socket = ArmsList[x - 1].ArmEnd.transform.position;
            }

            ArmX.Arm.transform.rotation *= Quaternion.Euler(-5f * (x + 1), (-2f * (-2 * x)), -6f);
            ArmX.Arm.transform.position =
                ArmX.Arm.transform.position -
                ArmX.ArmBase.transform.position +
                ArmX.Socket;

        }
    }
}
