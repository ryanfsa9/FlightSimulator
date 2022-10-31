using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Noise : MonoBehaviour
{
    public Transform airport;
    public float scale;
    public Vector2 seed;
    public Hills hills;
    public Ocean ocean;
    public Mountain mountain;
    public MountainMask mountainMask;
    Vector3 airportPos;
    float airportHeight;

    void Start()
    {
        airportPos = transform.InverseTransformPoint(airport.position);
        float xPos = airportPos.x*scale+seed.x;
        float zPos = airportPos.z*scale+seed.y;
        airportHeight = mountainMask.get(xPos,zPos) * mountain.get(xPos,zPos) + hills.get(xPos,zPos) - ocean.get(xPos,zPos);
        airport.transform.position = new Vector3(airport.transform.position.x,transform.TransformPoint(new Vector3(0,airportHeight,0)).y+0.01f,airport.transform.position.z);
    }
    public float[,] getHeights(Vector3 position){
        float[,] heights = new float[241,241];
        for(int x = 0; x<241; x++){
            for(int z = 0; z<241; z++){
                float xPos = (position.x+x/240f)*scale+seed.x;
                float zPos = (position.z+z/240f)*scale+seed.y;
                heights[x,z] = mountainMask.get(xPos,zPos) * mountain.get(xPos,zPos) + hills.get(xPos,zPos) - ocean.get(xPos,zPos);
                
                float distX = position.x + x/240f - airportPos.x;
                float distZ = position.z + z/240f - airportPos.z;
                float distance = new Vector2(5f*distX,distZ).magnitude;
                if(distance < 0.1f){
                    float bruh = Mathf.Clamp((0.1f-distance)*30f,0,1);
                    heights[x,z] = Mathf.Lerp(heights[x,z], airportHeight, bruh);
                }
                //heights[x,z] = mountainMask.get(xPos,zPos);
                //heights[x,z] = -ocean.get(xPos,zPos);
            }
        }
        return heights;
    }

    [System.Serializable]
    public class Hills{
        public float height;
        public float scale;
        public Vector2 seed;
        public float get(float x, float z){
            return Mathf.PerlinNoise(x*scale+seed.x,z*scale+seed.y)*height;
        }
    }
    [System.Serializable]
    public class Ocean{
        public float scale;
        public Vector2 seed;
        public float floor;
        public float ceiling;
        public float get(float x, float z){
            float val = Mathf.PerlinNoise(x*scale+seed.x,z*scale+seed.y);
            val -= floor;
            val *= ceiling;
            return Mathf.SmoothStep(0,1,val);
        }
    }
    [System.Serializable]
    public class Mountain{
        public float height;
        public float scale;
        public float power;
        public Vector2 seed;
        
        public int layers;
        public float persistance;
        public float lacunarity;
        public float get(float x, float z){
            float val = 0;
            float weight = 1;
            float layerHeight = 1;
            float layerScale = scale;
            Vector2 layerSeed = seed;
            for(int i = 0; i<layers; i++){
                val += Mathf.Pow(1f-Mathf.Abs(Mathf.PerlinNoise(x*layerScale+layerSeed.x,z*layerScale+layerSeed.y)-0.5f),power)*layerHeight*weight;
                weight = val;
                layerHeight *= persistance;
                layerScale *= lacunarity;
                layerSeed += new Vector2(18.3f,9.2f);
            }
            return val*height;
        }
    }
    [System.Serializable]
    public class MountainMask{
        public float scale;
        public Vector2 seed;
        public float floor;
        public float ceiling;
        public float get(float x, float z){
            float val = Mathf.PerlinNoise(x*scale+seed.x,z*scale+seed.y);
            val -= floor;
            val *= ceiling;
            return Mathf.SmoothStep(0,1,val);
        }
    }
}
