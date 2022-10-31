using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class UpdateThrd : MonoBehaviour
{
    public float[] LOD_Dists;
    meshGenThrd mgt;
    public int viewDistance;
    int viewDist;
    public Transform mainCam;
    float camX;
    float camZ;
    public Dictionary<Vector2,chunk> activeChunks;
    Queue<chunk> destroys = new Queue<chunk>();
    Queue<LODChange> LODs = new Queue<LODChange>();
    bool running;
    void Start()
    {
        viewDist = 2;
        running = true;
        for(int i = transform.childCount-1; i>=0; i--){
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        camX = transform.InverseTransformPoint(mainCam.position).x;
        camZ = transform.InverseTransformPoint(mainCam.position).z;
        activeChunks = new Dictionary<Vector2,chunk>();
        mgt = GetComponent<meshGenThrd>();
        new Thread(updateThread).Start();
    }
    void updateThread(){
        Debug.Log("run");
        while(running){
            for(int x = -viewDist-1; x<=viewDist+1; x++){
                for(int z = -viewDist-1; z<=viewDist+1; z++){
                    Vector2 chunkCoord = new Vector2(Mathf.RoundToInt(camX) + x, Mathf.RoundToInt(camZ) + z);
                    if((chunkCoord - new Vector2(camX,camZ)).magnitude < viewDist){
                        if(!activeChunks.ContainsKey(chunkCoord)){
                            mgt.requestChunk(chunkCoord);
                            activeChunks.Add(chunkCoord, null);
                        }
                    }
                    else{
                        if(activeChunks.ContainsKey(chunkCoord) && activeChunks[chunkCoord] != null){
                            destroys.Enqueue(activeChunks[chunkCoord]);
                            activeChunks.Remove(chunkCoord);
                        }
                    }
                }
            }
            foreach(chunk c in activeChunks.Values.ToList()){
                if(c != null){
                    float sqrDist = (new Vector2(camX,camZ) - c.pos).sqrMagnitude;
                    for(int i = 0; i<4; i++){
                        if(sqrDist < LOD_Dists[i] * LOD_Dists[i]){
                            if(c.LOD != i){
                                c.LOD = i;
                                LODs.Enqueue(new LODChange(c, i));
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        if(viewDist <= viewDistance){
            viewDist++;
        }
        camX = transform.InverseTransformPoint(mainCam.position).x;
        camZ = transform.InverseTransformPoint(mainCam.position).z;

        while(destroys.Count > 0){
            Destroy(destroys.Dequeue().gameObject);
        }
        while(LODs.Count > 0){
            LODs.Dequeue().apply();
        }
    }
    void OnDestroy(){
        running = false;
    }
    class LODChange{
        chunk c;
        int i;

        public LODChange(chunk cc, int ii){
            c = cc;
            i = ii;
        }
        public void apply(){
            c.mf.mesh = c.ms.meshArray[i];
            if(i == 0){
                c.mc.sharedMesh = c.ms.meshArray[0];
            }
            else{
                c.mc.sharedMesh = null;
            }
        }
    }
}
