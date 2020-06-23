using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKArmController : MonoBehaviour
{
    [System.Serializable]
    public struct ArmStruct
    {
        public GameObject ArmParent;  //Holds Y rotation (from base)
        [HideInInspector]
        public GameObject ArmObject;  //Holds Z rotation towards next point
        [HideInInspector]
        public GameObject ArmBase; //The base for moving the arm to the previous socket
        [HideInInspector]
        public GameObject ArmEnd; //The end of the arm; the next arms socket usually
        [HideInInspector]
        public GameObject ArmSocket; //The place it meets the previous arm/base
    };

    public ArmStruct[] ArmsList = new ArmStruct[0];

    public GameObject Target;
    public GameObject BaseObject;

    [Range(1, 20)]
    public int RotationClamp = 5;

    public bool bDrawDebugLines;
    public bool bInheritRotations = false;
    public bool bRotateBase = false;
    private int frame = 0;

    void Start()
    {
        //debug for empty array
        if (ArmsList.Length == 0)
        {
            Debug.LogError(gameObject.name + "'s ArmList is empty?");
            return;
        }

        //debug for empty target
        if (Target == null)
        {
            Debug.LogError(gameObject.name + "'s target is null?");
            return;
        }

        //set the base of the arm
        if (BaseObject == null)
        {
            Debug.LogWarning(gameObject.name +
                " has no BaseObject set; using itself as placeholder");
            BaseObject = gameObject;
        }

        //Set values for other objects in struct array
        for (int x = 0; x < ArmsList.Length; x++)
        {
            ArmsList[x].ArmObject = ArmsList[x].ArmParent.transform.Find("Arm").gameObject;
            ArmsList[x].ArmBase = ArmsList[x].ArmObject.transform.Find("ArmBase").gameObject;
            ArmsList[x].ArmEnd = ArmsList[x].ArmObject.transform.Find("ArmEnd").gameObject;


            if (x == 0)  //if the first arm in the array, set the base object as the socket.            
            {
                ArmsList[x].ArmSocket = BaseObject;
            }
            else //For other arms, set the socket to the previous base
            {
                ArmsList[x].ArmSocket = ArmsList[x - 1].ArmEnd;
            }

            Debug.Log("Arm " + ArmsList[x].ArmParent + "'s socket is " + ArmsList[x].ArmSocket);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        frame++;
        if(frame % 2 != 0) // if not multiple of 2
        {
            if (bRotateBase) { RotateBase(); }
            if (bInheritRotations) { InheritRotations(); }
            InverseArmRotation();
        }
        else { ForwardArmRotation(); }
        */
        
        if (bRotateBase) { RotateBase(); }
        if (bInheritRotations) { InheritRotations(); }
        InverseArmRotation();
        ForwardArmRotation();

    }

    private void RotateBase()
    {
        BaseObject.transform.Rotate(0, 1, 0);
    }

    private void OldCode()
    {
        //FirstArm.GetComponentInChildren<Transform>().Rotate(0, 0, 1);
        //Arm.transform.Rotate(0, 0, 1);

        //Debug.Log(FirstArm.GetComponentInChildren<Transform>().gameObject.name);

        ArmsList[0].ArmParent.transform.rotation = BaseObject.transform.rotation;

        BaseObject.transform.Rotate(0, 1, 0);

        //Vector3 ProjectedPoint = ProjectOntoPlane(Arm, ArmBase);
        Vector3 ProjectedPoint = ProjectOntoPlane(ArmsList[0].ArmObject, ArmsList[0].ArmBase, ArmsList[0].ArmParent, Target);

        Transform r = ArmsList[0].ArmBase.transform;

        float Angle = Vector3.Angle(r.up, ProjectedPoint - r.position);
        float SignAngle = Vector3.SignedAngle(r.up, ProjectedPoint - r.position, r.forward);

        ArmsList[0].ArmObject.transform.Rotate(0, 0, SignAngle);

        ArmsList[0].ArmObject.transform.position -= BaseObject.transform.position + ArmsList[0].ArmBase.transform.position;
    }

    private void InheritRotations()
    {
        for (int i = 0; i < ArmsList.Length; i++)
        {
            if(i == 0)
            {
                ArmsList[i].ArmParent.transform.rotation = BaseObject.transform.rotation;
            }
            else
            {
                ArmsList[i].ArmParent.transform.rotation = ArmsList[i - 1].ArmObject.transform.rotation;

                //TODO - This is a temp fix
                ArmsList[i].ArmParent.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }

        }
    }

    private void InverseArmRotation()
    {
        for (int i = ArmsList.Length - 1; i >= 0; i--)  //Reverse loop
        {
            if (i == ArmsList.Length - 1)   // if the final arm
            {
                PointAt(i, Target);
                
                ArmsList[i].ArmObject.transform.position -=
                ArmsList[i].ArmEnd.transform.position -
                Target.transform.position;
                
            }
            else                            // for other arms
            {
                PointAt(i, ArmsList[i + 1].ArmBase);
                
                ArmsList[i].ArmObject.transform.position -=
                ArmsList[i].ArmEnd.transform.position -
                ArmsList[i + 1].ArmBase.transform.position;
                
            }

            
        }
    }

    private void ForwardArmRotation()
    {
        for (int i = 0; i < ArmsList.Length; i++)
        {
            //ArmData value used to shorten code
            ArmStruct ArmData = ArmsList[i];

            //move back to sockets without rotation
            ArmData.ArmObject.transform.position =
            ArmData.ArmObject.transform.position -
            ArmData.ArmBase.transform.position +
            ArmData.ArmSocket.transform.position;
        }
    }

    public void PointAt(int x, GameObject TargetObject)
    {
        //Get the aim position for the next frame
        Vector3 ProjectedPoint = ProjectOntoPlane(ArmsList[x].ArmObject, ArmsList[x].ArmBase, ArmsList[x].ArmParent, TargetObject);

        //NEW allign the arm with armparent
        //ArmsList[x].ArmObject.transform.rotation = ArmsList[x].ArmParent.transform.rotation;

        //Rotate to the aim position
        Transform r = ArmsList[x].ArmBase.transform;    //TODO
        float SignAngle = Vector3.SignedAngle(r.up, ProjectedPoint - r.position, r.forward);

        SignAngle = Mathf.Clamp(SignAngle, -RotationClamp, RotationClamp);

        ArmsList[x].ArmObject.transform.Rotate(0, 0, SignAngle/2);

        //Move to the next position
        //ArmsList[0].ArmObject.transform.position -= BaseObject.transform.position + ArmsList[0].ArmBase.transform.position;
        //ArmsList[0].ArmObject.transform.position -= BaseObject.transform.position + ArmsList[0].ArmBase.transform.position;
    }

    public Vector3 ProjectOntoPlane(GameObject ArmObject, GameObject ArmBase, GameObject ArmParent, GameObject ArmTarget)
    {
        Transform r = ArmBase.transform;                //TODO
        /*
        Plane p = new Plane(
            r.position,
            r.position + ArmObject.transform.up,
            r.position + ArmObject.transform.right);
        */
        Plane p = new Plane(
            r.position,
            r.position + ArmParent.transform.up,
            r.position + ArmParent.transform.right);
        
        //normal is cross product of Arm.transform.up and Arm.transform.right
        Vector3 TargetToOrigin = ArmTarget.transform.position - r.position;
        float NormalDistance = Vector3.Dot(TargetToOrigin, p.normal);
        Vector3 ProjectedPoint = ArmTarget.transform.position - (p.normal * NormalDistance);

        //draw debug lines?
        if (bDrawDebugLines) { DrawDebugLines(r, p, ProjectedPoint); }

        return ProjectedPoint;
    }

    void DrawDebugLines(Transform r, Plane p, Vector3 ProjectedPoint)
    {
        Debug.DrawLine(r.position, r.position + r.up, Color.blue);
        Debug.DrawLine(r.position, r.position + r.right, Color.cyan);

        Debug.DrawRay(r.position, p.normal, Color.red);
        //Debug.DrawLine(r.position, Target.transform.position, Color.green);

        Debug.DrawLine(r.position, ProjectedPoint, Color.yellow);
    }
}



