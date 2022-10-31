using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;
using UnityEngine.UI;

public class flightScript : NetworkBehaviour
{
    float health;
    public float speed;
    public float thrust;
    public float banking;
    public bool gearDown;
    public float brakeStrength;
    Vector3 spawnPos;
    CameraScript camS;

    Vector3 force;
    public Rigidbody rb;
    Animation wheelAnimation;
    public float controlSpeed;
    enemyHUD hud;
    Slider healthBar;

    void Awake()
    {
        health = 1f;
        hud = GetComponent<enemyHUD>();
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        spawnPos = GameObject.FindGameObjectsWithTag("Respawn")[0].transform.position;
        transform.position = spawnPos;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.Find("CoM").localPosition;
        wheelAnimation = GetComponent<Animation>();
        camS = GameObject.Find("Camera").GetComponent<CameraScript>();
        
        rb.velocity = new Vector3(0.01f,0.01f,0.01f);
        rb.angularVelocity = new Vector3(0.01f,0.01f,0.01f);
    }

    void Update()
    {
        rb.centerOfMass = transform.Find("CoM").localPosition;

        speed = transform.InverseTransformDirection(rb.velocity).z * 0.75f;

        if(Input.GetKeyDown("g")){
            if(wheelAnimation["Scene"].time == 0 && !gearDown){wheelAnimation["Scene"].normalizedTime = 1;}
            if(gearDown){wheelAnimation["Scene"].speed =  1; wheelAnimation.Play("Scene"); gearDown = false;}
            else        {wheelAnimation["Scene"].speed = -1; wheelAnimation.Play("Scene"); gearDown =  true;}
        }
        if(Input.GetKeyDown("r")){
            CmdDie();
        }
        if(Input.GetKeyDown("escape")){
            Application.Quit();
        }
        hud.CmdSetHealth(health);
        healthBar.normalizedValue = health;
        if(health <= 0f){
            CmdDie();
        }
    }
    IEnumerator respawn(){
        yield return new WaitForSeconds(3f);
        health = 1f;
        hud.CmdSetHealth(health);
        transform.position = spawnPos;
        transform.rotation = new Quaternion();
        rb.velocity = transform.TransformDirection(new Vector3(0,0,20f));
        transform.Find("explosion").GetComponent<VisualEffect>().enabled = false;
        camS.bias = camS.orgBias;
    }
    [Command]
    void CmdDie(){
        RpcDie();
    }
    [ClientRpc]
    void RpcDie(){
        if(isLocalPlayer){
            camS.bias = 0;
        }
        transform.Find("explosion").GetComponent<VisualEffect>().enabled = true;
        StartCoroutine(respawn());
    }
    [ClientRpc]
    public void RpcHit(){
        health -= 0.5f;
    }
    [ClientRpc]
    public void RpcShot(){
        health -= 0.025f;
    }
    void OnDrawGizmos()
    {
        //Gizmos.color = new Color(1f,0f,0,1f);
        //Gizmos.DrawLine(rudder.tf.position, rudder.tf.position + rudder.tf.TransformDirection(new Vector3(0f,rudder.axis.y,rudder.axis.x)));

        //Gizmos.DrawLine(ailerons.tf.position, ailerons.tf.position + ailerons.tf.TransformDirection(new Vector3(ailerons.axis.x,0f,ailerons.axis.y)));
    }
}
