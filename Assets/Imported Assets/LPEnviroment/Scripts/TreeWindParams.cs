using UnityEngine;
namespace LPEnviroment
{
public class TreeWindParams : MonoBehaviour
{
    //public Texture2D perlin;
    [Range(0,1.5f)]
    public float magnitude=1, magnitudeTurbulence=1;
    [Range(-Mathf.PI,Mathf.PI)]
    public float direction=0, directionTurbulence=1;
    [Range(0.01f,5.0f)]
    public float transitionTime=1;
    Vector4 MmDd;
    int windMainId, tNoiseId;
    Shader s;

    /*[DebugGUIGraph(min:0,max:2.0f,r:1,g:0,b:0,autoScale:true,group:1)] float outMod;
    [DebugGUIGraph(min:0,max:2.0f,r:0,g:1,b:0,autoScale:true,group:1)] float outDir;*/

    void Start(){
        MmDd      = new Vector4(magnitude, magnitudeTurbulence, direction, directionTurbulence);
        s         = Shader.Find("Shader Graphs/LPTree");
        windMainId= Shader.PropertyToID("_MmDd");
        tNoiseId  = Shader.PropertyToID("_Wind_time");
        
        //DebugGUI.SetGraphProperties("ff", "ff", 0, Mathf.PI*2.0f, 1, new Color(0, 0, 0), true);
    }
    /* HLSL port
    float perlinAdapt(float t){return 1-Mathf.PerlinNoise1D(t);}
    Vector2 float2(float a, float b){return new Vector2(a,b);}
    Vector3 float3(float a, float b, float c){return new Vector3(a,b,c);}
    Vector4 float4(float a, float b, float c, float d){return new Vector4(a,b,c,d);}
    
    float sin(float a){return Mathf.Sin(a);}
    float cos(float a){return Mathf.Cos(a);}
    float min(float a,float b){return Mathf.Min(a,b);}
    float max(float a,float b){return Mathf.Max(a,b);}
    Vector3 SAMPLE_TEXTURE2D_LOD(Texture2D tex,object ss,Vector2 uv,int lod){
        return (Vector4)tex.GetPixelBilinear(Mathf.PingPong(uv.x,1), Mathf.PingPong(uv.y,1),lod);
    }*/

    Vector3 t=new Vector3(0,0,0);
    void Update(){
        Vector4 newMmDd= new Vector4(magnitude, magnitudeTurbulence, direction, directionTurbulence);
        MmDd=Vector4.Lerp(MmDd,newMmDd,Time.deltaTime/transitionTime);

        float  dt = Mathf.Min(0.1f,Time.deltaTime);
        t.x += dt;
        t.y += dt * Mathf.Max(0.5f, MmDd.x+MmDd.y+0.2f);
        t.z += dt * Mathf.Min(2.0f,1.5f*MmDd.y);

        Shader.SetGlobalVector(windMainId,MmDd);
        Shader.SetGlobalVector(tNoiseId,t);
    }
}
}
