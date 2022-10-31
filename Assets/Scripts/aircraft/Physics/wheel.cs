using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheel : MonoBehaviour
{
    WheelCollider wc;
    float brakes;

    void Start()
    {
        wc = GetComponent<WheelCollider>();
        wc.ConfigureVehicleSubsteps(1,12,15);
        float speed = GetComponentInParent<flightScript>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("b")){wc.brakeTorque = GetComponentInParent<flightScript>().brakeStrength;}
        else                 {wc.brakeTorque =     0f; wc.motorTorque = 1f;}
    }
}
