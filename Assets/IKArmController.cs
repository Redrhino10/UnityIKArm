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
        public GameObject SocketPosition; //The place it meets the previous arm/base
        //[HideInInspector]
        //public Quaternion SocketRotation;  //The rotation of the previous object to inherit
    };

    public ArmStruct[] ArmsList = new ArmStruct[0];

    public GameObject Target;
    public GameObject BaseObject;
    //public GameObject FirstArm;
    //private GameObject Arm, ArmBase;
    //private float previousAngle, currentAngle;

    

    public bool bDrawDebugLines;

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
        }

        //Arm = FirstArm.transform.GetChild(0).gameObject;
        //ArmBase = Arm.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //FirstArm.GetComponentInChildren<Transform>().Rotate(0, 0, 1);
        //Arm.transform.Rotate(0, 0, 1);

        //Debug.Log(FirstArm.GetComponentInChildren<Transform>().gameObject.name);
        ArmsList[0].ArmParent.transform.rotation = BaseObject.transform.rotation;

        BaseObject.transform.Rotate(0, 1, 0);

        //Vector3 ProjectedPoint = ProjectOntoPlane(Arm, ArmBase);
        Vector3 ProjectedPoint = ProjectOntoPlane(ArmsList[0].ArmObject, ArmsList[0].ArmBase);

        Transform r = ArmsList[0].ArmBase.transform;

        float Angle = Vector3.Angle(r.up, ProjectedPoint - r.position);
        float SignAngle = Vector3.SignedAngle(r.up, ProjectedPoint - r.position, r.forward);

        ArmsList[0].ArmObject.transform.Rotate(0, 0, SignAngle);

        ArmsList[0].ArmObject.transform.position -= BaseObject.transform.position + ArmsList[0].ArmBase.transform.position;
    }



    public Vector3 ProjectOntoPlane(GameObject ArmObject, GameObject ArmBase)
    {
        Transform r = ArmBase.transform;
        Plane p = new Plane(
            r.position,
            r.position + ArmObject.transform.up,
            r.position + ArmObject.transform.right);


        //normal is cross product of Arm.transform.up and Arm.transform.right
        Vector3 TargetToOrigin = Target.transform.position - r.position;
        float NormalDistance = Vector3.Dot(TargetToOrigin, p.normal);
        Vector3 ProjectedPoint = Target.transform.position - (p.normal * NormalDistance);

        if (bDrawDebugLines) { DrawDebugLines(r, p, ProjectedPoint); }

        return ProjectedPoint;
    }

    void DrawDebugLines(Transform r, Plane p, Vector3 ProjectedPoint)
    {
        Debug.DrawLine(r.position, r.position + r.up, Color.blue);
        Debug.DrawLine(r.position, r.position + r.right, Color.cyan);

        Debug.DrawRay(r.position, p.normal, Color.red);
        Debug.DrawLine(r.position, Target.transform.position, Color.green);

        Debug.DrawLine(r.position, ProjectedPoint, Color.yellow);
    }
}



