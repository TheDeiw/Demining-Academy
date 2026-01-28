using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.EditorTools;
//using UnityEditor.BuildTargetDiscovery;
namespace LPEnviroment
{
[EditorTool("LPEnviroment visualization")]
class LPEnviromentTool : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    Texture2D m_ToolIcon;
    Vector3[] ellipse=new Vector3[64];

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "LPEnviroment visualization",
            tooltip = "LPEnviroment shader ellipsoid visualization"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    void DrawEllipse(Vector3[] pts, Vector3 pos,Vector3 ext, int nrmAxis)
    {
        int x=(nrmAxis+1)%3, z=(nrmAxis+2)%3;
        float a=0, step=(Mathf.PI*2)/(pts.Length-1);
        for(int i=0;i<pts.Length;i++){
            pts[i][x]=Mathf.Sin(a)*ext[x];
            pts[i][z]=Mathf.Cos(a)*ext[z];
            pts[i][nrmAxis]=0;
            pts[i]+=pos;
            a+=step;
        }
        UnityEngine.Rendering.CompareFunction bkp=Handles.zTest;
        Color bkpc=Handles.color;
        Handles.zTest=UnityEngine.Rendering.CompareFunction.Less;
        Handles.DrawPolyLine(pts);
        Handles.zTest=UnityEngine.Rendering.CompareFunction.Greater;
        Handles.color=new Color(1,1,1,0.2f);
        Handles.DrawPolyLine(pts);
        Handles.zTest=bkp;
        Handles.color=bkpc;
    }
    
    void DrawEllipse(GameObject g, Shader[] filt)
    {
          MeshRenderer mr=g.GetComponent<MeshRenderer>();
          if(mr==null){return;}
          Material[] ms=mr.sharedMaterials;

          foreach(Material m in ms){
              foreach(Shader s in filt){
                if(m.shader==s){
                  Vector3 ellipsoidPos=(Vector3)m.GetVector("_EllipsoidCenter")+g.transform.position;
                  Vector4 ellipsoidExt=m.GetVector("_EllipsoidDef");

                  DrawEllipse(ellipse, ellipsoidPos, ellipsoidExt,0);
                  DrawEllipse(ellipse, ellipsoidPos, ellipsoidExt,1);
                  DrawEllipse(ellipse, ellipsoidPos, ellipsoidExt,2);    
                }
              }
          }
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {
        Shader[] trgs=new Shader[]{
            Shader.Find("Nature/TreeSRP"), Shader.Find("Nature/TreeSRPLegacy"), 
            Shader.Find("Nature/TreeURP"),
            Shader.Find("Shader Graphs/LPTree"), Shader.Find("Shader Graphs/TreeHDRP")
        };
        if(Selection.gameObjects.Length==0){return;}
        DrawEllipse(Selection.gameObjects[0], trgs);
        for(int i=0;i < Selection.gameObjects[0].transform.childCount; i++){
            DrawEllipse(Selection.gameObjects[0].transform.GetChild(i).gameObject, trgs);
        }
    }
}
}