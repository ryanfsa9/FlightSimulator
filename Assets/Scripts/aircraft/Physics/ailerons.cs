using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ailerons : MonoBehaviour
{
    public Transform leftAileron;
    public Transform rightAileron;
    public Vector3 position;
    public Vector2 size;
    public float baseLift;
    public Vector2 axis;
    Rigidbody rb;
    Vector3 rightPos;
    float bankForce;
    Vector3 force_L;
    Vector3 force_R;

    public float turnSpeed;
    public float maxAngle;
    public AnimationCurve speedCurve;
    float angle_L;
    float angle_R;
    Vector3 origPos;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        bankForce = GetComponentInParent<flightScript>().banking;
        rightPos = new Vector3(-position.x,position.y,position.z);
        origPos = leftAileron.localPosition;
    }
    void Update()
    {
        float sc = speedCurve.Evaluate(transform.parent.InverseTransformDirection(rb.velocity).z);

        if(Input.GetKey("q")){angle_L -= turnSpeed*Time.deltaTime; angle_R -= turnSpeed*Time.deltaTime;}
        if(Input.GetKey("e")){angle_L += turnSpeed*Time.deltaTime; angle_R += turnSpeed*Time.deltaTime;}
        if(Input.GetMouseButton(1)){angle_L += Input.GetAxis("Mouse X")/10f; angle_R += Input.GetAxis("Mouse X")/10f;}
        if(Input.GetKey("b")){angle_L += turnSpeed*Time.deltaTime/sc*2; angle_R -= turnSpeed*Time.deltaTime/sc*2;}
        
        angle_L = Mathf.Clamp(angle_L,-1f,1f);
        angle_L -= angle_L*turnSpeed*Time.deltaTime;
        angle_R = Mathf.Clamp(angle_R,-1f,1f);
        angle_R -= angle_R*turnSpeed*Time.deltaTime;

        Vector3 direction = leftAileron.TransformDirection(new Vector3(axis.x,0f,axis.y));
        leftAileron.localRotation = new Quaternion();
        leftAileron.RotateAround(leftAileron.position,direction,angle_L*maxAngle*sc);
        leftAileron.localPosition = origPos;

        direction = rightAileron.TransformDirection(new Vector3(-axis.x,0f,axis.y));
        rightAileron.localRotation = new Quaternion();
        rightAileron.RotateAround(rightAileron.position,direction,angle_R*maxAngle*sc);
        rightAileron.localPosition = new Vector3(-origPos.x,origPos.y,origPos.z);
    }
    void FixedUpdate()
    {
        force_L = calcWing(position, leftAileron);
        force_R = calcWing(rightPos, rightAileron);
    }
    Vector3 calcWing(Vector3 pos, Transform tf){
        Vector3 vel = rb.velocity + Vector3.Cross(rb.angularVelocity, tf.TransformPoint(pos) - tf.parent.TransformPoint(rb.centerOfMass));
        vel = tf.InverseTransformDirection(vel);
        vel.x = 0;

        float crossSection = Mathf.Abs(size.x*size.y * Mathf.Cos(Mathf.Atan(vel.y/vel.z)));
        //Standard Lift
        Vector3 localForce = new Vector3(0f, baseLift * size.x * size.y * vel.z, 0f);
        //Banking
        localForce += new Vector3(0f, -2f * bankForce * crossSection * vel.y, 0f);

        rb.AddForceAtPosition(tf.TransformDirection(localForce), tf.TransformPoint(pos), ForceMode.Acceleration);
        return localForce;
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,0f,1f,1f);
        Gizmos.DrawLine(leftAileron.TransformPoint(position), leftAileron.TransformPoint(position+force_L/5f));
        Gizmos.DrawLine(rightAileron.TransformPoint(rightPos), rightAileron.TransformPoint(rightPos+force_R/5f));
        drawWing(position, leftAileron);
        drawWing(rightPos, rightAileron);
    }
    void drawWing(Vector3 pos, Transform tf)
    {
        Vector3 point_TL = tf.TransformPoint(new Vector3(pos.x + size.x/2, pos.y, pos.z + size.y/2));
        Vector3 point_TR = tf.TransformPoint(new Vector3(pos.x - size.x/2, pos.y, pos.z + size.y/2));
        Vector3 point_BL = tf.TransformPoint(new Vector3(pos.x + size.x/2, pos.y, pos.z - size.y/2));
        Vector3 point_BR = tf.TransformPoint(new Vector3(pos.x - size.x/2, pos.y, pos.z - size.y/2));
        Gizmos.DrawLine(point_TL,point_TR);
        Gizmos.DrawLine(point_TR,point_BR);
        Gizmos.DrawLine(point_BR,point_BL);
        Gizmos.DrawLine(point_BL,point_TL);
    }
        
}
