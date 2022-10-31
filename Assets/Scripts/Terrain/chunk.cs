using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class chunk : MonoBehaviour
{
    public meshGenThrd.meshes ms;
    public int LOD;
    public MeshFilter mf;
    public MeshCollider mc;
    public Vector2 pos;

    public void initialize(meshGenThrd.meshes m, Vector2 p){
        ms = m;
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
        pos = p;
    }
}
