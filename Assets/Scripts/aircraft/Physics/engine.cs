using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class engine : MonoBehaviour
{
    public float thrust;
    public AnimationCurve thrustCurve;
    public float throttleSpeed;
    flightScript script;
    Rigidbody rb;
    public float throttle;
    public RectTransform handle;
    public VisualEffect vfx;
    public float plumeLength;
    public AudioSource sound;

    void Start()
    {
        throttle = 0f;
        rb = GetComponent<Rigidbody>();
        script = GetComponent<flightScript>();
        handle = GameObject.Find("Handle").GetComponent<RectTransform>();
    }

    void Update()
    {
        if(Input.GetKey("left shift")){throttle += throttleSpeed*Time.deltaTime;}
        if(Input.GetKey("left ctrl" )){throttle -= throttleSpeed*Time.deltaTime;}
        if(Input.GetKey("z")){throttle = 1f;}
        if(Input.GetKey("x")){throttle = 0f;}

        throttle = Mathf.Clamp(throttle,0f,1f);

        handle.anchoredPosition = new Vector2(-370f,130*throttle-120f);

        vfx.SetFloat("Length", throttle * thrustCurve.Evaluate(script.speed) * plumeLength);
        sound.volume = throttle * thrustCurve.Evaluate(script.speed) * 0.5f;
    }
    void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * throttle * thrust * thrustCurve.Evaluate(script.speed), ForceMode.Acceleration);
    }
}
