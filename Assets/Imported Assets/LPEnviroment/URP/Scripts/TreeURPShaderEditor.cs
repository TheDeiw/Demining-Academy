#if UNITY_2020_1_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;//.Rendering.Universal.ShaderGUI
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal.ShaderGUI;

namespace LPEnviroment
{
class TreeURPShaderEditor : BaseShaderGUI
{
    static readonly string[] workflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));
    private static class Styles2
    {
        public static GUIContent ellipsoidDefText    = EditorGUIUtility.TrTextContent("Ellipsoid definition", "Ellipsoid definition");
        public static GUIContent ellipsoidCenterText = EditorGUIUtility.TrTextContent("Ellipsoid center",     "Ellipsoid center");
        public static GUIContent farShadowText       = EditorGUIUtility.TrTextContent("Far shadow fade",      "Far shadow fade");
        public static GUIContent windNoiseText       = EditorGUIUtility.TrTextContent("Wind noise",           "Wind noise");
        public static GUIContent windWeightsText     = EditorGUIUtility.TrTextContent("Wind weights (LFTB)",  "Wind weights (LFTB)");
        public static GUIContent coreSizeText        = EditorGUIUtility.TrTextContent("Core size",            "Core size"); 
    }
    MaterialProperty ellipsoidDef  = null;
    MaterialProperty ellipsoidCenter = null;
    MaterialProperty farShadow     = null;
    MaterialProperty windNoise     = null;
    MaterialProperty windWeights   = null;
    MaterialProperty coreSize      = null;

    private LitGUI.LitProperties litProperties;
    /*private LitDetailGUI.LitProperties litDetailProperties;

    public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
    {
        materialScopesList.RegisterHeaderScope(LitDetailGUI.Styles.detailInputs, Expandable.Details, _ => LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor));
    }*/

    // collect properties from the material properties
    public override void FindProperties(MaterialProperty[] props)
    {
        base.FindProperties(props);
        litProperties = new LitGUI.LitProperties(props);
        //litDetailProperties = new LitDetailGUI.LitProperties(props);
        
        ellipsoidDef = FindProperty("_EllipsoidDef", props);
        ellipsoidCenter = FindProperty("_EllipsoidCenter", props);
        farShadow   = FindProperty("_FarShadow", props);
        windNoise   = FindProperty("_WindNoise", props);
        windWeights = FindProperty("_WindWeights", props);
        coreSize    = FindProperty("_CoreSize", props);
    }

    // material changed check
    public override void ValidateMaterial(Material material)
    {
        SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, null);//LitDetailGUI.SetMaterialKeywords);
    }

    // material main surface options
    public override void DrawSurfaceOptions(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        if (litProperties.workflowMode != null)
            DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, workflowModeNames);

        base.DrawSurfaceOptions(material);
    }

    // material main surface inputs
    public override void DrawSurfaceInputs(Material material)
    {
        UniversalRenderPipelineAsset urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        base.DrawSurfaceInputs(material);
        LitGUI.Inputs(litProperties, materialEditor, material);
        DrawEmissionProperties(material, true);
        DrawTileOffset(materialEditor, baseMapProp);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wind & Shading settings", EditorStyles.boldLabel);
        float s0=EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, s1=10;
        Rect r=GUILayoutUtility.GetLastRect();                                               r.y+=s0;
        materialEditor.ShaderProperty(r, ellipsoidDef,    Styles2.ellipsoidDefText.text);    r.y+=s0;
        materialEditor.ShaderProperty(r, ellipsoidCenter, Styles2.ellipsoidCenterText.text); r.y+=s0+s1;
        GUILayout.Space(s0*3+s1*2);

        materialEditor.ShaderProperty(r, farShadow,       "Far shadow fade");       r.y+=s0;
        EditorGUI.LabelField(r, "Shadow distance: "+urp.shadowDistance);
        GUILayout.Space(s0);
        
        materialEditor.TexturePropertySingleLine(  Styles2.windNoiseText, windNoise);
        r=GUILayoutUtility.GetLastRect();                                           r.y+=s0;
        materialEditor.ShaderProperty(r,windWeights, Styles2.windWeightsText.text); r.y+=s0;
        materialEditor.ShaderProperty(r,coreSize,    Styles2.coreSizeText.text);    r.y+=s0;
        GUILayout.Space(s0*2);
    }

    // material main advanced options
    public override void DrawAdvancedOptions(Material material)
    {
        if (litProperties.reflections != null && litProperties.highlights != null)
        {
            materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
            materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
        }

        base.DrawAdvancedOptions(material);
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        if (material == null){throw new ArgumentNullException("material");}

        // _Emission property is lost after assigning Standard shader to the material
        // thus transfer it before assigning the new shader
        if (material.HasProperty("_Emission"))
        {
            material.SetColor("_EmissionColor", material.GetColor("_Emission"));
        }

        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
        {
            SetupMaterialBlendMode(material);
            return;
        }

        SurfaceType surfaceType = SurfaceType.Opaque;
        BlendMode blendMode = BlendMode.Alpha;
        if (oldShader.name.Contains("/Transparent/Cutout/"))
        {
            surfaceType = SurfaceType.Opaque;
            material.SetFloat("_AlphaClip", 1);
        }
        else if (oldShader.name.Contains("/Transparent/"))
        {
            // NOTE: legacy shaders did not provide physically based transparency
            // therefore Fade mode
            surfaceType = SurfaceType.Transparent;
            blendMode = BlendMode.Alpha;
        }
        material.SetFloat("_Blend", (float)blendMode);

        material.SetFloat("_Surface", (float)surfaceType);
        if (surfaceType == SurfaceType.Opaque)
        {
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else
        {
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        if (oldShader.name.Equals("Standard (Specular setup)"))
        {
            material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
            Texture texture = material.GetTexture("_SpecGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }
        else
        {
            material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
            Texture texture = material.GetTexture("_MetallicGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }
    }
}
}
#endif