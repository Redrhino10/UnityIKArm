using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKArmController : MonoBehaviour
{
    public GameObject Target;
    public GameObject BaseObject;
    public GameObject FirstArm;
    private GameObject Arm, ArmBase;
    private bool bPositiveRotation = true;
    //private float previousAngle, currentAngle;

    public bool bDrawDebugLines;

    void Start()
    {
        Arm = FirstArm.transform.GetChild(0).gameObject;
        ArmBase = Arm.transform.GetChild(0).gameObject;     
    }

    // Update is called once per frame
    void Update()
    {
        //FirstArm.GetComponentInChildren<Transform>().Rotate(0, 0, 1);
        //Arm.transform.Rotate(0, 0, 1);

        //Debug.Log(FirstArm.GetComponentInChildren<Transform>().gameObject.name);
        FirstArm.transform.rotation = BaseObject.transform.rotation;

        BaseObject.transform.Rotate(0, 1, 0);

        Vector3 ProjectedPoint = ProjectOntoPlane(Arm, ArmBase);

        Transform r = ArmBase.transform;

        float Angle = Vector3.Angle(r.up, ProjectedPoint - r.position);
        float SignAngle = Vector3.SignedAngle(r.up, ProjectedPoint - r.position, r.forward);

        Arm.transform.Rotate(0, 0, SignAngle);

        Arm.transform.position -= BaseObject.transform.position + ArmBase.transform.position;
    }



    public Vector3 ProjectOntoPlane(GameObject Arm, GameObject ArmBase)
    {
        Transform r = ArmBase.transform;
        Plane p = new Plane(
            r.position,
            r.position + Arm.transform.up,
            r.position + Arm.transform.right);


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



