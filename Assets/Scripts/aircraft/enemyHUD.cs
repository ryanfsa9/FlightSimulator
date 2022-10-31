using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class enemyHUD : NetworkBehaviour
{
    Camera cam;
    RectTransform rect;
    RectTransform canv;
    public GameObject prefabHUD;
    Rigidbody rb;
    Rigidbody playerRb;

    [SyncVar]
    string playerName;
    [SyncVar]
    float health;
    Text tx;
    Slider sl;
    RectTransform lead;
    public InputField inputField;
    void Awake()
    {
        health = 1f;
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        canv = GameObject.Find("Canvas").GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody>();
        
        GameObject go = GameObject.Instantiate(prefabHUD, canv.transform);
        rect = go.GetComponent<RectTransform>();
        tx = go.transform.Find("NameTag").GetComponent<Text>();
        sl = go.transform.Find("Slider").GetComponent<Slider>();
        lead = go.transform.Find("Lead").GetComponent<RectTransform>();
        lead.parent = canv;
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
    }

    void Update()
    {
        if(isLocalPlayer){
            CmdChangeName(inputField.text);
        }
        else{
            playerRb = NetworkClient.localPlayer.GetComponent<Rigidbody>();
            rect.anchoredPosition = getScreenPoint(transform.position);
            tx.text = playerName;
            sl.normalizedValue = health;

            float t = (transform.position - playerRb.transform.position).magnitude / 5000f;
            Vector3 leadPos = transform.position + t * rb.velocity;
            lead.anchoredPosition = getScreenPoint(leadPos);
        }
    }


    [Command]
    public void CmdChangeName(string newName){
        playerName = newName;
    }
    [Command]
    public void CmdSetHealth(float h){
        health = h;
    }

    Vector2 getScreenPoint(Vector3 pos){
        if(cam.transform.InverseTransformPoint(pos).z < 0){return new Vector2(5000,5000);}
        Vector2 ViewportPosition=cam.WorldToViewportPoint(pos);
        return new Vector2(((ViewportPosition.x*canv.sizeDelta.x)-(canv.sizeDelta.x*0.5f)),((ViewportPosition.y*canv.sizeDelta.y)-(canv.sizeDelta.y*0.5f)));
    }
}
