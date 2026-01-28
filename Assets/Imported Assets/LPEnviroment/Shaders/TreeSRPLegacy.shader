Shader "Nature/TreeSRPLegacy"
{
    Properties {
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 1.0
        _Color ("Main Color", Color) = (1,1,1,1)
        [MainTexture]_MainTex ("Texture", 2D) = "white" {}
        //[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        
        [Header(Wind and Shading settings)]
        _EllipsoidDef("Leaves bound extent and density",   Vector) = (4.5,4.5,4.5,0.25)
        _EllipsoidCenter("Leaves bound position", Vector) = (0,5.5,0)
        _FarShadow("Far shadow", Vector) = (125,1,0)
        
        /*_Color ("Main Color", Color) = (1,1,1,0)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _HalfOverCutoff ("0.5 / Alpha cutoff", Range(0,1)) = 1.0
        _BaseLight ("Base Light", Range(0, 1)) = 0.35
        _AO ("Amb. Occlusion", Range(0, 10)) = 2.4
        _Occlusion ("Dir Occlusion", Range(0, 20)) = 7.5*/
        
        [NoScaleOffset]_WindNoise("Wind noise", 2D) = "grey" {}
        _WindWeights("Wind weights (LFTB)", Vector) = (0.5,0.5,0.5,0.5)
        _CoreSize("Core size", Float) = 0.2
        
        // These are here only to provide default values
        [HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
        [HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
        [HideInInspector] _SquashAmount ("Squash", Float) = 1
    }
    SubShader
    {
        Cull[_Cull]
        Pass
        {
            Tags {"LightMode"="ForwardBase" /*"Queue" = "AlphaTest"*/}// Fog desn!t work in AlphaTest
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "TerrainEngine.cginc"
            
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_fog
            
            sampler2D _MainTex;
            half4 _Color;
            half _Cutoff;

            struct v2f
            {
                float4 uv_d_fs : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                half4 ambient_nl : COLOR0;
                half4 ambientb_f: COLOR1;
                //float3 posWorld: COLOR2;
                //half3 nrmWorld : COLOR3;
                float4 pos : SV_POSITION;
            };
            
            #define TREE_BASE_LEGACY
            #define TREE_BASE_A
            #include "TreeBase.cginc"
            
            v2f vert (appdata_base v)
            {
                v.vertex.xyz   *= _TreeInstanceScale.xyz;
                half3 nrmWorld  = UnityObjectToWorldNormal(v.normal);
                float3 posWorld = mul(unity_ObjectToWorld, v.vertex);
                v2f o           = (v2f)0;
                o.ambient_nl    = half4(ShadeSH9(half4( nrmWorld,1)), dot(nrmWorld,_WorldSpaceLightPos0.xyz));
                o.ambientb_f.rgb= ShadeSH9(half4(-nrmWorld,1));
                vertTree(o, v.vertex, posWorld, v.texcoord.xy);
                
                //TREE_TRANSFER_FOG(o.pos.z, o.ambientb_f.w);
                UNITY_CALC_FOG_FACTOR((o.pos.z));
                o.ambientb_f.w  = unityFogFactor;

                TRANSFER_SHADOW(o)
                return o;
            }

            half4 frag (v2f i, bool isFF:SV_IsFrontFace) : SV_Target
            {   
                half4 col  = tex2D(_MainTex, i.uv_d_fs.xy)*_Color*_TreeInstanceColor;
                if(col.a   < _Cutoff){discard;}
                half atten = SHADOW_ATTENUATION(i);
                if(!isFF){i.ambient_nl  = half4(i.ambientb_f.rgb, -i.ambient_nl.a);}
                
                half diff  = max(0, i.ambient_nl.a);
                diff       = treeDiffuse(diff, i.uv_d_fs.z, i.uv_d_fs.w, atten)*atten;
                col.rgb    = col.rgb * (i.ambient_nl.rgb + _LightColor0.rgb * diff);

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                UNITY_FOG_LERP_COLOR(col.rgb,unity_FogColor,i.ambientb_f.w);
#endif
                return col;
            }
            ENDCG
        }
    
        Pass
        {
            Tags {"LightMode"="ForwardAdd" "Queue" = "AlphaTest"}
            Blend One One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "TerrainEngine.cginc"

            // (we don't care about any lightmaps yet, so skip these variants)
            //#pragma multi_compile_fwdadd nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile_fwdadd_fullshadows
            
            sampler2D _MainTex;
            half4 _Color;
            half _Cutoff;

            struct v2f
            {
                float4 uv_d_fs : TEXCOORD0;
                //UNITY_SHADOW_COORDS(1)  // declare shadows data - it works even without it, wtf?
                //DECLARE_LIGHT_COORDS(2) // declare shadows data - it works even without it, wtf?
                //UNITY_LIGHTING_COORDS(1,2) // replaces the two above
                half nl: COLOR0;
                float3 posWorld: COLOR2;
                //half3 nrmWorld : COLOR3;
                float4 pos : SV_POSITION;
            };

            #define TREE_BASE_LEGACY
            #define TREE_BASE_A
            #include "TreeBase.cginc"
            

            v2f vert (appdata_base v)
            {
                v.vertex.xyz  *= _TreeInstanceScale.xyz;
                half3 nrmWorld = UnityObjectToWorldNormal(v.normal);
                v2f o;
                o.posWorld     = mul(unity_ObjectToWorld, v.vertex);
                o.nl           = dot(nrmWorld, UnityWorldSpaceLightDir(o.posWorld));
                vertTree(o, v.vertex, o.posWorld, v.texcoord.xy);
                
                // compute shadows data - it works even without it, wtf?
                //TRANSFER_SHADOW(o);//UNITY_TRANSFER_SHADOW(o);
                //COMPUTE_LIGHT_COORDS(o)
                return o;
            }

            half4 frag (v2f i, bool isFF:SV_IsFrontFace) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv_d_fs.xy)*_Color*_TreeInstanceColor;
                if(col.a < _Cutoff){discard;}
                UNITY_LIGHT_ATTENUATION(atten, i, i.posWorld)
                if(!isFF){i.nl = -i.nl;}
                
                atten*=0.5;// Adjust towards Standard shader
                half diff     = max(0, i.nl);// * _LightColor0.rgb;
                diff = treeDiffuse(diff, i.uv_d_fs.z, i.uv_d_fs.w, atten)*atten;
                col.rgb = col.rgb * (_LightColor0.rgb * diff);
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"

            sampler2D _MainTex;
            half4 _Color;
            half _Shininess;
            half _Cutoff;

            #define TREE_BASE_LEGACY_SHADOW
            #define TREE_BASE_A
            #include "TreeBase.cginc"

            struct v2f { 
                float2 uv : TEXCOORD0;
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v.vertex.xyz      *= _TreeInstanceScale.xyz;
                float3 posWorld    = mul(unity_ObjectToWorld, v.vertex);
                half3 windMovement = treeWindSway(_MmDd, posWorld-unity_ObjectToWorld._m03_m13_m23, unity_ObjectToWorld._m03_m13_m23, _EllipsoidCenter, _EllipsoidDef.xyz, _WindWeights, _CoreSize, _Wind_time, 1);
                v.vertex          += mul(unity_WorldToObject,float4(windMovement,0));
                v2f o;
                o.uv               = v.texcoord;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                if(col.a < _Cutoff){discard;}
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    //Dependency "BillboardShader" = "Hidden/Nature/Tree Soft Occlusion Leaves Rendertex"
    //Dependency "BillboardShader" = "Hidden/Nature/TreeSRPLegacyBillboard"
}
