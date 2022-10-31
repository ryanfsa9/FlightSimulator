using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public weapons shooter;

    void OnCollisionEnter(Collision col){
        print("bruh");
        if (col.gameObject.tag == "Player")
        {
            if (shooter != null && col.gameObject != shooter.gameObject)
            {
                if (shooter == null || col.gameObject == shooter.gameObject)
                {
                    print("selfHit");
                }
                print("hit");
                shooter.RpcShotHitMark();
                col.gameObject.GetComponent<flightScript>().RpcShot();

            }
        }
        Destroy(this.gameObject);
    }
}
