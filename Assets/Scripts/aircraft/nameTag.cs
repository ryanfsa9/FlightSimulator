using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/*public class nameTag : NetworkBehaviour
{
    [SyncVar]
    public string nm;
    Camera cam;
    RectTransform rt;
    Text tx;
    RectTransform canv;
    public GameObject nameTagPrefab;
    public InputField inputField;

    void Start()
    {
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        canv = GameObject.Find("Canvas").GetComponent<RectTransform>();
        GameObject nt = GameObject.Instantiate(nameTagPrefab, canv.transform);
        rt = nt.GetComponent<RectTransform>();
        tx = nt.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer){
            CmdChangeName(inputField.text);
        }
        else{
            rt.anchoredPosition = getScreenPoint(transform.position) + new Vector2(0,28);
            tx.text = nm;
        }
    }
    [Command]
    public void CmdChangeName(string newName){
        nm = newName;
    }


    Vector2 getScreenPoint(Vector3 pos){
        if(cam.transform.InverseTransformPoint(pos).z < 0){return new Vector2(5000,5000);}
        Vector2 ViewportPosition=cam.WorldToViewportPoint(pos);
        return new Vector2(((ViewportPosition.x*canv.sizeDelta.x)-(canv.sizeDelta.x*0.5f)),((ViewportPosition.y*canv.sizeDelta.y)-(canv.sizeDelta.y*0.5f)));
    }
}*/
