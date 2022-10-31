using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class startUp : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        print("started");
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach(MonoBehaviour script in scripts){
            script.enabled = true;
        }
        scripts = GetComponentsInChildren<MonoBehaviour>();
        foreach(MonoBehaviour script in scripts){
            script.enabled = true;
        }
        Transform cam = GameObject.Find("Camera").transform;
        cam.GetComponent<CameraScript>().plane = transform;
        cam.GetComponent<CameraScript>().point = transform.Find("Follow Point");
    }
}
