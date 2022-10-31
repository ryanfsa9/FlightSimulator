using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform point;
    public Transform plane;
    public float bias;
    [HideInInspector]
    public float orgBias;
    Camera cam;
    Quaternion planeView;
    Quaternion crosshairView;
    float t;

    // Update is called once per frame
    void Start(){
        t = 0;
        orgBias = bias;
        cam = GetComponent<Camera>();
        plane = GameObject.Find("Cube").transform;
    }
    void Update()
    {
        planeView = Quaternion.LookRotation(plane.position-transform.position);
        crosshairView = Quaternion.LookRotation(plane.TransformPoint(new Vector3(0,0,100000))-transform.position);
        if(Input.GetMouseButton(0)){
            t = Mathf.Lerp(t,1,5*Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 15f, 4f * Time.deltaTime);
        }
        else{
            t = Mathf.Lerp(t,0,5*Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, 4f * Time.deltaTime);
        }
        transform.rotation = Quaternion.Slerp(planeView,crosshairView,t);
    }
    void FixedUpdate(){
        if(point != null){
            GetComponent<Rigidbody>().MovePosition(transform.position + (point.position - transform.position + new Vector3(0f,5f,0f)) * bias);
        }
    }
}
