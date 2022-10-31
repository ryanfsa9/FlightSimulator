using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wings : MonoBehaviour
{
    public Vector3 position;
    public Vector2 size;
    public float baseLift;
    Rigidbody rb;
    Vector3 rightPos;
    float bankForce;
    Vector3 force_L;
    Vector3 force_R;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        bankForce = GetComponentInParent<flightScript>().banking;
        rightPos = new Vector3(-position.x,position.y,position.z);
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        force_L = calcWing(position);
        force_R = calcWing(rightPos);
    }
    Vector3 calcWing(Vector3 pos){
        Vector3 vel = rb.velocity + Vector3.Cross(rb.angularVelocity, transform.TransformPoint(pos) - transform.parent.TransformPoint(rb.centerOfMass));
        vel = transform.InverseTransformDirection(vel);
        vel.x = 0;

        float crossSection = Mathf.Abs(size.x*size.y * Mathf.Cos(Mathf.Atan(vel.y/vel.z)));
        //Standard Lift
        Vector3 localForce = new Vector3(0f, baseLift * size.x * size.y * vel.z, 0f);
        //Banking
        localForce += new Vector3(0f, -2f * bankForce * crossSection * vel.y, 0f);

        rb.AddForceAtPosition(transform.TransformDirection(localForce), transform.TransformPoint(pos), ForceMode.Acceleration);
        return localForce;
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,0f,1f,1f);
        Gizmos.DrawLine(transform.TransformPoint(position), transform.TransformPoint(position+force_L/5f));
        Gizmos.DrawLine(transform.TransformPoint(rightPos), transform.TransformPoint(rightPos+force_R/5f));
        drawWing(position);
        drawWing(rightPos);
    }
    void drawWing(Vector3 pos)
    {
        Vector3 point_TL = transform.TransformPoint(new Vector3(pos.x + size.x/2, pos.y, pos.z + size.y/2));
        Vector3 point_TR = transform.TransformPoint(new Vector3(pos.x - size.x/2, pos.y, pos.z + size.y/2));
        Vector3 point_BL = transform.TransformPoint(new Vector3(pos.x + size.x/2, pos.y, pos.z - size.y/2));
        Vector3 point_BR = transform.TransformPoint(new Vector3(pos.x - size.x/2, pos.y, pos.z - size.y/2));
        Gizmos.DrawLine(point_TL,point_TR);
        Gizmos.DrawLine(point_TR,point_BR);
        Gizmos.DrawLine(point_BR,point_BL);
        Gizmos.DrawLine(point_BL,point_TL);
    }
        
}
