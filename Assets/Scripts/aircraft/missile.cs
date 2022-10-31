using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Mirror;

public class missile : NetworkBehaviour
{
    public weapons shooter;
    public weapons targetScript;
    Rigidbody rb;
    public float speed;
    public float tracking;
    public float maxLife;
    public Transform target;
    public Transform tTemp;
    float life;

    void Awake(){
        
        rb = GetComponent<Rigidbody>();
        life = maxLife;
        transform.GetChild(0).gameObject.GetComponent<VisualEffect>().enabled = true;
        transform.GetChild(1).gameObject.GetComponent<TrailRenderer>().enabled = true;
        target = null;
        StartCoroutine(enableCollider());
    }
    void Update(){
        if(targetScript != null && target != null && transform.InverseTransformPoint(target.position).z > 0){
            targetScript.RpcMissileWarn((target.position - transform.position).magnitude);
        }
    }
    void FixedUpdate(){
        rb.AddRelativeForce(new Vector3(0,0,speed));
        float s = transform.InverseTransformDirection(rb.velocity).z;
        
        if(target != null){
            Vector3 targetPos = target.position - transform.position;
            float angleDiff = Vector3.Angle(transform.forward, targetPos);
            Vector3 cross = Vector3.Cross(transform.forward, targetPos).normalized;
            rb.AddTorque(cross * tracking, ForceMode.Acceleration);
        }

        rb.velocity = transform.TransformDirection(new Vector3(0,0,s));
        life -= Time.deltaTime;

        if(life < 0){
            explode();
        }
    }
    void OnCollisionEnter(Collision col){
        if(maxLife - life > 0.5f)
        {
            GetComponent<CapsuleCollider>().enabled = false;
            if(target != null && col.gameObject == target.gameObject){
                shooter.RpcMissileHitMark();
                target.gameObject.GetComponent<flightScript>().RpcHit();
            }
            explode();
        }
    }
    void explode(){
        transform.GetChild(2).gameObject.GetComponent<VisualEffect>().enabled = true;
        transform.GetChild(0).gameObject.GetComponent<VisualEffect>().enabled = false;
        rb.velocity = new Vector3();
        rb.isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(stopExplode());
        StartCoroutine(destroyCo());
    }
    IEnumerator enableCollider(){
        yield return new WaitForSeconds(0.25f);
        GetComponent<CapsuleCollider>().enabled = true;
        target = tTemp;
    }
    IEnumerator stopExplode(){
        yield return new WaitForSeconds(1f);
        transform.GetChild(2).gameObject.GetComponent<VisualEffect>().Stop();
    }
    IEnumerator destroyCo(){
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}