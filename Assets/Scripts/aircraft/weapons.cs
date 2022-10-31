using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class weapons : NetworkBehaviour
{
    public GameObject missilePrefab;
    public GameObject bulletPrefab;
    public GameObject serverBulletPrefab;
    public GameObject[] missileHolders;
    public GameObject gun;
    
    int missileNum;
    Transform target;
    int activeWeapon;
    bool firing;
    public float bulletSpeed;
    public float bloom;
    float closestMissile;
   
    RectTransform tracker;
    RectTransform firePoint;
    Color hitMarkerColor;
    Image hitMarker;
    Camera cam;
    RectTransform canv;
    float hitMarkerAlpha;
    public AudioSource gunSound;
    public AudioSource warning;
    float warnTime;
    float disableTime;

    void Start()
    {
        hitMarkerAlpha = 0;
        warnTime = 0;
        disableTime = 0;
        missileNum = 0;
        target = null;
        hitMarker = GameObject.Find("HitMarker").GetComponent<Image>();
        tracker = GameObject.Find("Tracker").GetComponent<RectTransform>();
        firePoint = GameObject.Find("FirePoint").GetComponent<RectTransform>();
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        canv = GameObject.Find("Canvas").GetComponent<RectTransform>();
    }

    public void launchMissile(){
        if(activeWeapon == 0 && missileHolders[missileNum].active){
            CmdLaunch(missileHolders[missileNum].transform.position, missileHolders[missileNum].transform.rotation, target);
            missileHolders[missileNum].active = false;
            StartCoroutine(reActivate(missileNum));
            missileNum++;
            if(missileNum == missileHolders.Length){
                missileNum = 0;
            }
        }
    }
    [Command]
    void CmdLaunch(Vector3 pos, Quaternion rot, Transform t){
        GameObject m = (GameObject) Instantiate(missilePrefab, pos, rot);
        NetworkServer.Spawn(m);
        m.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        m.GetComponent<missile>().tTemp = t;
        m.GetComponent<missile>().shooter = this;
        if(t != null){
            m.GetComponent<missile>().targetScript = t.gameObject.GetComponent<weapons>();
        }
    }
    [Command]
    void CmdFire(Vector3 pos, Quaternion rot){
        GameObject bc = Instantiate(serverBulletPrefab, pos, rot);
        bc.GetComponent<bullet>().shooter = this;
        bc.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.TransformDirection(new Vector3(Random.Range(-bloom,bloom),Random.Range(-bloom,bloom),bulletSpeed));
        Destroy(bc,5);
        RpcFire(pos,rot);
    }
    [ClientRpc]
    void RpcFire(Vector3 pos, Quaternion rot){
        GameObject b = Instantiate(bulletPrefab, pos, rot);
        b.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.TransformDirection(new Vector3(Random.Range(-bloom,bloom),Random.Range(-bloom,bloom),bulletSpeed));
        Destroy(b,3);
    }
    [ClientRpc]
    public void RpcShotHitMark(){
        hitMarkerColor = new Color(1f,1f,1f,1f);
        hitMarkerAlpha = 1f;
    }
    [ClientRpc]
    public void RpcMissileHitMark(){
        hitMarkerColor = new Color(0.753f,0f,0f,2f);
        hitMarkerAlpha = 2f;
    }
    [ClientRpc]
    public void RpcMissileWarn(float dist){
        if(dist < closestMissile){
            closestMissile = dist;
            disableTime = 0.35f;
        }
    }
    void Update(){
        firePoint.anchoredPosition = getScreenPoint(transform.TransformPoint(new Vector3(0f,0f,100000f)));
        if(Input.GetKeyDown("space")){launchMissile();}
        firing = (Input.GetKey("space"))? true : false;
        if(Input.GetKeyDown("tab")){activeWeapon = (activeWeapon == 1)? activeWeapon = 0 : activeWeapon+1;}

        target = null;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject go in gos){
            if(go.transform != target && go.transform != transform){
                Vector3 lp = transform.InverseTransformPoint(go.transform.position).normalized;
                if(lp.z > 0 && Mathf.Sqrt(lp.x*lp.x*lp.y*lp.y) < 0.2f){
                    if(target == null || (go.transform.position - transform.position).sqrMagnitude  <  (target.position - transform.position).sqrMagnitude){
                        target = go.transform;
                    }
                }
            }
        }
        if(target != null){
            tracker.anchoredPosition = getScreenPoint(target.position);
        }
        else{
            tracker.anchoredPosition = firePoint.anchoredPosition;
        }
        hitMarkerColor.a = hitMarkerAlpha;
        hitMarker.color = hitMarkerColor;
        hitMarkerAlpha -= 1f*Time.deltaTime;

        if(disableTime > 0){
            warnTime += 5000f/closestMissile * Time.deltaTime;
            disableTime -= Time.deltaTime;
            if(warnTime >= 1f){
                warning.Play();
                warnTime = 0f;
            }
        }
        else{
            closestMissile = 3000f;
        }
    }
    void FixedUpdate(){
        if(firing && activeWeapon == 1){
            gunSound.enabled = true;
            CmdFire(gun.transform.position, transform.rotation);
        }
        else{
            gunSound.enabled = false;
        }
    }
    Vector2 getScreenPoint(Vector3 pos){
        Vector2 ViewportPosition=cam.WorldToViewportPoint(pos);
        return new Vector2(((ViewportPosition.x*canv.sizeDelta.x)-(canv.sizeDelta.x*0.5f)),((ViewportPosition.y*canv.sizeDelta.y)-(canv.sizeDelta.y*0.5f)));
    }
    IEnumerator reActivate(int i){
        yield return new WaitForSeconds(20);
        missileHolders[i].active = true;
    }
}
