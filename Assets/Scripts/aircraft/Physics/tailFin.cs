using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tailFin : MonoBehaviour
{
    public Vector3 position;
    public Vector2 size;
    public float baseLift;
    Rigidbody rb;
    float bankForce;
    Vector3 force;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        bankForce = GetComponentInParent<flightScript>().banking;
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        force = calcWing(position);
    }
    Vector3 calcWing(Vector3 pos){
        Vector3 vel = rb.velocity + Vector3.Cross(rb.angularVelocity, transform.TransformPoint(pos) - transform.parent.TransformPoint(rb.centerOfMass));
        vel = transform.InverseTransformDirection(vel);
        vel.y = 0;

        float crossSection = Mathf.Abs(size.x*size.y * Mathf.Cos(Mathf.Atan(vel.x/vel.z)));
        //Standard Lift
        Vector3 localForce = new Vector3(0f, baseLift * size.x * size.y * vel.z, 0f);
        //Banking
        localForce += new Vector3(-2f * bankForce * crossSection * vel.x, 0f, 0f);

        rb.AddForceAtPosition(transform.TransformDirection(localForce), transform.TransformPoint(pos), ForceMode.Acceleration);
        return localForce;
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f,0f,1f,1f);
        Gizmos.DrawLine(transform.TransformPoint(position), transform.TransformPoint(position+force/5f));
        drawWing(position);
    }
    void drawWing(Vector3 pos)
    {
        Vector3 point_TL = transform.TransformPoint(new Vector3(pos.x, pos.y + size.y/2, pos.z + size.x/2));
        Vector3 point_TR = transform.TransformPoint(new Vector3(pos.x, pos.y - size.y/2, pos.z + size.x/2));
        Vector3 point_BL = transform.TransformPoint(new Vector3(pos.x, pos.y + size.y/2, pos.z - size.x/2));
        Vector3 point_BR = transform.TransformPoint(new Vector3(pos.x, pos.y - size.y/2, pos.z - size.x/2));
        Gizmos.DrawLine(point_TL,point_TR);
        Gizmos.DrawLine(point_TR,point_BR);
        Gizmos.DrawLine(point_BR,point_BL);
        Gizmos.DrawLine(point_BL,point_TL);
    }
        
}
