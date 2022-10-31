using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class meshGenThrd : MonoBehaviour
{
    public GameObject chunkObj;
    UpdateThrd ut;
    bool running;
    Noise noise;
    Queue<Vector2> requests = new Queue<Vector2>();
    Queue<meshDatas> create = new Queue<meshDatas>();

    void Start()
    {
        ut = GetComponent<UpdateThrd>();
        noise = GetComponent<Noise>();
        running = true;
        Thread thr = new Thread(meshGenThread);
        thr.Start();
    }
    public void requestChunk(Vector2 pos){
        requests.Enqueue(pos);
    }
    void meshGenThread(){
        while(running){
            if(requests.Count > 0){
                Vector2 pos = requests.Dequeue();
                create.Enqueue(calcMeshes(pos));
            }
        }
        Debug.Log("quit");
    }
    meshDatas calcMeshes(Vector2 pos){
        meshDatas mds = new meshDatas();
        float[,] heights = noise.getHeights(new Vector3(pos.x,0,pos.y));
        calcMesh(0,240);
        calcMesh(1,120);
        calcMesh(2,60);
        calcMesh(3,15);
        mds.pos = pos;

        return mds;

        void calcMesh(int m, int res){
            mds.vertsArray[m] = new Vector3[(res+1) * (res+1)];
            mds.trisArray[m] = new int[6 * res * res];
            mds.uvsArray[m] = new Vector2[(res+1) * (res+1)];

            int i = 0;
            int ii = 0;
            for(int x = 0; x<=res; x++){
                for(int z = 0; z<=res; z++){
                    Vector3 location = new Vector3(x/(float)res, heights[x*240/res,z*240/res], z/(float)res);
                    mds.vertsArray[m][i] = location;
                    mds.uvsArray[m][i] = new Vector2(location.x,location.z);
                    if(x!=res && z!=res){
                        mds.trisArray[m][ii  ] = i;
                        mds.trisArray[m][ii+1] = i + 1;
                        mds.trisArray[m][ii+2] = i + res+1;
                        mds.trisArray[m][ii+3] = i + 1;
                        mds.trisArray[m][ii+4] = i + res+2;
                        mds.trisArray[m][ii+5] = i + res+1;
                        ii += 6;
                    }
                    i++;
                }
            }
        }
    }

    void Update(){
        if(create.Count > 0){
            for(int i = 0; i<create.Count; i++){
                meshDatas mds = create.Dequeue();
                GameObject obj = GameObject.Instantiate(chunkObj, transform.TransformPoint(new Vector3(mds.pos.x,0,mds.pos.y)), new Quaternion(), transform);
                meshes ms = new meshes();
                for(int ii = 0; ii<=3; ii++){
                    ms.meshArray[ii] = new Mesh();
                    ms.meshArray[ii].vertices = mds.vertsArray[ii];
                    ms.meshArray[ii].triangles = mds.trisArray[ii];
                    ms.meshArray[ii].uv = mds.uvsArray[ii];
                    ms.meshArray[ii].RecalculateNormals();
                }
                chunk c = obj.GetComponent<chunk>();
                c.initialize(ms,mds.pos);
                ut.activeChunks[mds.pos] = c;
            }
        }
    }

    void OnDestroy(){
        running = false;
    }

    public class meshes{
        public Mesh[] meshArray;
        public meshes(){
            meshArray = new Mesh[4];
        }
    }
    public class meshDatas{
        public Vector3[][] vertsArray;
        public int[][] trisArray;
        public Vector2[][] uvsArray;
        public Vector2 pos;
        public meshDatas(){
            vertsArray = new Vector3[4][];
            trisArray = new int[4][];
            uvsArray = new Vector2[4][];
        }
    }
}