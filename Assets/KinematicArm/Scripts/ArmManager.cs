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
        //[HideInInspector]
        //public Quaternion SocketRotation;  //The rotation of the previous object to inherit
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
                //ArmsList[x].SocketRotation = BaseObject.transform.rotation;
            }
            else
            {
                ArmsList[x].SocketPosition = ArmsList[x - 1].ArmEnd.transform.position;
                //ArmsList[x].SocketRotation = ArmsList[x - 1].ArmEnd.transform.rotation;
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
            //Debug.Log("Checking int i = " + i);

            if (i == ArmsList.Length - 1)   // if the final arm
            {
                //PointAt(Tracker.transform, i, Tracker);  //... then face the tracker
                //PointTowards(ArmsList[i], Tracker.gameObject, ArmsList[i - 1].Arm);
                PointAtNew(ArmsList[i], Tracker.gameObject, Tracker.gameObject, ArmsList[i - 1].Arm);
            }
            else if(i == 0)   //NEW for the first arm
            {
                //PointTowards(ArmsList[i], ArmsList[i + 1].ArmBase, BaseObject);
                PointAtNew(ArmsList[i], ArmsList[i + 1].ArmBase, ArmsList[i + 1].Arm, BaseObject);
            }
            else                        // for other arms
            {
                //PointAt(ArmsList[i + 1].ArmBase.transform, i, ArmsList[i+1].Arm);
                //PointTowards(ArmsList[i], ArmsList[i + 1].ArmBase, ArmsList[i - 1].Arm);
                PointAtNew(ArmsList[i], ArmsList[i + 1].ArmBase, ArmsList[i + 1].Arm, ArmsList[i - 1].Arm);
            }
        }
    }

    void PointAt(Transform AimPosition, int ArmData, GameObject AimObject)
    {
        //face the next object
        //NOTE* commented out since this is the main solution, but doesnt work well
        //ArmsList[ArmData].Arm.transform.LookAt(AimPosition);

        //NOTE* this code below works MUCH better, but may need tinkering!
        //test to move to midpoint of current and target position
        Vector3 target = ArmsList[ArmData].ArmEnd.transform.position;
        target += (AimPosition.transform.position - target) / 2;
        ArmsList[ArmData].Arm.transform.LookAt(target);

        //We want to limit the rotation to an angle (ie 90degrees)
        //need angle between the arm and the aim position
        Debug.DrawLine(ArmsList[ArmData].Arm.transform.position,
            target, Color.red, 1);

        Debug.DrawLine(AimPosition.position,
            AimObject.transform.position, Color.blue, 1);


        float angle = Vector3.Angle(ArmsList[ArmData].Arm.transform.position - target,
            AimPosition.position - AimObject.transform.position);
        if (angle > 90)
        {
            Debug.Log(string.Format("Angle between {0} and {1} is {2}",
            ArmsList[ArmData].Arm, AimObject, angle));
        }
        


        /*
        Vector3 x = ArmsList[ArmData].Arm.transform.position;
        
        Vector3 ArmDirection = transform.TransformDirection(target - x).normalized;
        Vector3 AimDirection = transform.TransformDirection(AimPosition.position - x).normalized;

        float DotValue = Vector3.Dot(ArmDirection, AimDirection);

        if (DotValue < 0)
        {
            Debug.Log("The arm point is behind! on " + ArmsList[ArmData].Arm.name);
        }
        else
        {
            Debug.Log(ArmsList[ArmData].Arm.name + ": Dot = " + DotValue);
        }
        */

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

    void PointTowards(ArmInfo CurrentArmInfo, GameObject NextBase, GameObject PreviousObject) //A new refined method for PointAt, which uses Euler angles instead to limit degrees of motion
    {
        //Debug lines
        Debug.DrawRay(CurrentArmInfo.Arm.transform.position, CurrentArmInfo.Arm.transform.forward, Color.red, 1);
        //Debug.DrawRay(NextBase.transform.position, NextBase.transform.forward, Color.blue, 1);

        // Get the aim position for next frame
        Vector3 target = CurrentArmInfo.ArmEnd.transform.position;
        target += (NextBase.transform.position - target) / 2;

        Vector3 PlaneNormal = Vector3.Cross(CurrentArmInfo.Arm.transform.forward, NextBase.transform.forward).normalized;

        Debug.DrawRay(CurrentArmInfo.ArmEnd.transform.position, PlaneNormal, Color.green, 1);

        float angle = Vector3.Angle(CurrentArmInfo.Arm.transform.forward, PreviousObject.transform.forward);

        Quaternion quaternion = Quaternion.FromToRotation(CurrentArmInfo.Arm.transform.forward, PreviousObject.transform.forward);

        //Vector3 angleaxis = Quaternion.AngleAxis(angle, PlaneNormal) * CurrentArmInfo.Arm.transform.position;
        Vector3 QuaternionAngle = quaternion * CurrentArmInfo.Arm.transform.forward;
        float EulerAngle = Quaternion.Angle(quaternion, Quaternion.Inverse(CurrentArmInfo.Arm.transform.rotation));
        Debug.Log(string.Format("{0}: {1}. {2}", CurrentArmInfo.Arm.name, angle, EulerAngle));

        //Debug.DrawLine(CurrentArmInfo.Arm.transform.position, QuaternionAngle, Color.black, 1);
        Debug.DrawRay(CurrentArmInfo.Arm.transform.position, QuaternionAngle, Color.black, 1);

        Vector3 newtarget = Quaternion.AngleAxis(0, PlaneNormal) * CurrentArmInfo.Arm.transform.forward;

        //Debug.Log(angleaxis.ToString() + ".... and " + target.ToString());
        Debug.DrawLine(target, newtarget, Color.white, 1);

        if (angle > 90)
        {
            Debug.Log("Arm " + CurrentArmInfo.Arm.name + " is over 90");
            //CurrentArmInfo.Arm.transform.LookAt(newtarget);

        }
        else
        {
            CurrentArmInfo.Arm.transform.LookAt(target);
        }
        

        //Move to next position
        CurrentArmInfo.Arm.transform.position -=
        CurrentArmInfo.ArmEnd.transform.position -
        NextBase.transform.position;
    }

    void PointAtNew(ArmInfo CurrentArmInfo, GameObject NextBase, GameObject NextArm, GameObject PreviousObject) //RETRY
    {
        
        // Get the aim position for next frame
        Vector3 target = CurrentArmInfo.ArmEnd.transform.position;
        target += (NextBase.transform.position - target) / 2;

        //float angle = Vector3.Angle(CurrentArmInfo.Arm.transform.forward, PreviousObject.transform.forward);
        //angle = Mathf.Clamp(angle, 0, 90);

        float angle2 = Vector3.Angle(NextArm.transform.forward, CurrentArmInfo.Arm.transform.forward);
        angle2 = Mathf.Clamp(angle2, 0, 90);

        Debug.DrawRay(CurrentArmInfo.Arm.transform.position, CurrentArmInfo.Arm.transform.forward, Color.black, 1);


        Debug.Log(string.Format("Angles of {0} is {1}", CurrentArmInfo.Arm.name, angle2));
        
        Vector3 PlaneNormal = Vector3.Cross(CurrentArmInfo.Arm.transform.forward, NextArm.transform.forward).normalized;
        Debug.DrawRay(CurrentArmInfo.ArmBase.transform.position, PlaneNormal, Color.green, 1);

        //Vector3 NewDirection = Quaternion.AngleAxis(angle, PlaneNormal) * CurrentArmInfo.Arm.transform.forward;
        Vector3 NewDirection2 = Quaternion.AngleAxis(angle2, PlaneNormal) * CurrentArmInfo.Arm.transform.forward;
        //Debug.DrawRay(CurrentArmInfo.Arm.transform.position, NewDirection, Color.blue, 1);
        Debug.DrawRay(CurrentArmInfo.Arm.transform.position, NewDirection2, Color.white, 1);
        Debug.DrawLine(CurrentArmInfo.Arm.transform.position, target, Color.cyan, 1);

        if (angle2 > 90)
        {
            Debug.Log("Arm " + CurrentArmInfo.Arm.name + " is over 90");  
        }


        //setup coordinate system
        Vector3 YVector = CurrentArmInfo.Arm.transform.forward;
        Vector3 ZVector = PlaneNormal;
        Vector3 XVector = Vector3.Cross(YVector, ZVector);

        Vector3 lookPos = target;

        CurrentArmInfo.Arm.transform.LookAt(target);

        if (CurrentArmInfo.Arm.GetComponent<ArmRestrictions>().rotateX == false)
        {
            CurrentArmInfo.Arm.transform.localEulerAngles = new Vector3(
                PreviousObject.transform.eulerAngles.x,
                CurrentArmInfo.Arm.transform.eulerAngles.y,
                CurrentArmInfo.Arm.transform.eulerAngles.z);
        }
        if (CurrentArmInfo.Arm.GetComponent<ArmRestrictions>().rotateY == false)
        {
            CurrentArmInfo.Arm.transform.localEulerAngles = new Vector3(
                CurrentArmInfo.Arm.transform.eulerAngles.x,
                PreviousObject.transform.eulerAngles.y,
                CurrentArmInfo.Arm.transform.eulerAngles.z);
        }
        if (CurrentArmInfo.Arm.GetComponent<ArmRestrictions>().rotateZ == false)
        {
            CurrentArmInfo.Arm.transform.localEulerAngles = new Vector3(
                CurrentArmInfo.Arm.transform.eulerAngles.x,
                CurrentArmInfo.Arm.transform.eulerAngles.y,
                PreviousObject.transform.eulerAngles.z);
        }

        var rotationxyz = Quaternion.LookRotation(lookPos);
        //CurrentArmInfo.Arm.transform.rotation = rotationxyz;

        //var delta = target - CurrentArmInfo.Arm.transform.position;
        //var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        //var rotation = Quaternion.Euler(0, angle, 0);


        

        //Move to next position
        CurrentArmInfo.Arm.transform.position -=
        CurrentArmInfo.ArmEnd.transform.position -
        NextBase.transform.position;
    }
}
