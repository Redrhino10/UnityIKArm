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
        public Vector3 SocketPosition; //The place it meets the previous arm/base
        [HideInInspector]
        public Quaternion SocketRotation;  //The rotation of the previous object to inherit
    };

    private float Timer = 0.0f;
    
    public ArmInfo[] ArmsList = new ArmInfo[0];
    [Tooltip("Leave this field blank to use this object as the origin/base.")]
    public GameObject BaseObject;

    [Tooltip("Tracker for IK")]
    public GameObject Tracker;

    // Start is called before the first frame update
    void Start()
    {
        //debug for empty array
        if(ArmsList.Length == 0)
        {
            Debug.LogError(gameObject.name + "'s ArmList is empty?");
            return;
        }

        //debug for empty tracker
        if (Tracker == null)
        {
            Debug.LogError(gameObject.name + "'s Tracker is empty?");
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
            if (x == 0)
            {
                ArmsList[x].SocketPosition = BaseObject.transform.position;
                ArmsList[x].SocketRotation = BaseObject.transform.rotation;
            }
            else
            {
                ArmsList[x].SocketPosition = ArmsList[x - 1].ArmEnd.transform.position;
                ArmsList[x].SocketRotation = ArmsList[x - 1].ArmEnd.transform.rotation;
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
        //timer to improve frame rate. probably not needed
        /*
        Timer += Time.deltaTime;
        if (Timer >= 0.01f)
        {
            //RotateTheArm();
            //PointAt(Tracker.transform, 2);
            InverseRotateArm();
            ForwardRotateArm();

            Timer -= 0.01f;
        }
        */
        InverseRotateArm();
        ForwardRotateArm();
    }

    void RotateTheArm()
    {
        //test code
        Quaternion InheritedRotation = new Quaternion(0,0,0,1);

        
        for (int i = 0; i < ArmsList.Length; i++)
        {
            ArmInfo ArmData = ArmsList[i];

            //set socket positions
            if(i==0) { ArmData.SocketPosition = BaseObject.transform.position; }
            else { ArmData.SocketPosition = ArmsList[i-1].ArmEnd.transform.position; }

            //rotate the arm
            Quaternion TestRotator = Quaternion.Euler(-1f, 4f, 6f);
            InheritedRotation *= TestRotator;
            Debug.Log("InheritedRotation of " + ArmData.Arm.name + ": "+ InheritedRotation.ToString());

            ArmData.Arm.transform.rotation *= InheritedRotation;

            //move the arm
            ArmData.Arm.transform.position =
                ArmData.Arm.transform.position -
                ArmData.ArmBase.transform.position +
                ArmData.SocketPosition;
        }
    }

    void ForwardRotateArm()
    {
        for (int i = 0; i < ArmsList.Length; i++)
        {
            //ArmData value used to shorten code
            ArmInfo ArmData = ArmsList[i];

            //set socket positions
            if (i == 0) { ArmData.SocketPosition = BaseObject.transform.position; }
            else { ArmData.SocketPosition = ArmsList[i - 1].ArmEnd.transform.position; }

            //move back to sockets without rotation
            ArmData.Arm.transform.position =
            ArmData.Arm.transform.position -
            ArmData.ArmBase.transform.position +
            ArmData.SocketPosition;
        }
    }

    void InverseRotateArm()
    {
        for (int i = ArmsList.Length - 1; i >= 0; i--)  //Reverse loop
        {
            Debug.Log("Checking int i = " + i);

            if (i == ArmsList.Length - 1)   // if the final arm
            {
                PointAt(Tracker.transform, i);  //... then face the tracker
            }
            else                        // for other arms
            {
                PointAt(ArmsList[i + 1].ArmBase.transform, i);
            }
        }
    }

    void PointAt(Transform AimPosition, int ArmData)
    {
        //face the next object
        //NOTE* commented out since this is the main solution, but doesnt work well
        //ArmsList[ArmData].Arm.transform.LookAt(AimPosition);

        //NOTE* this code below works MUCH better, but may need tinkering!
        //test to move to midpoint of current and target position
        Vector3 target = ArmsList[ArmData].ArmEnd.transform.position;
        target += (AimPosition.transform.position - target) / 2;
        ArmsList[ArmData].Arm.transform.LookAt(target);

        //move the arm but INVERSE!

        if (ArmData == ArmsList.Length - 1) // if the final element, aim for the tracker
        {
            //Debug.Log(ArmsList[ArmData].Arm.name);
            ArmsList[ArmData].Arm.transform.position -=
               ArmsList[ArmData].ArmEnd.transform.position -
               Tracker.transform.position; 
        } 
        else
        {
            ArmsList[ArmData].Arm.transform.position -=
            ArmsList[ArmData].ArmEnd.transform.position -
            ArmsList[ArmData + 1].ArmBase.transform.position;
        }
        
    }
}
