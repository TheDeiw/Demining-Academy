#ifndef TREE_BASE_INCLUDED
#define TREE_BASE_INCLUDED

#if !defined(TREE_BASE_B) && !defined(TREE_BASE_A)
    #warning "Tree occlusion mode not selected. TREE_BASE_A or TREE_BASE_B needs to be defined"
#endif

half4 _EllipsoidDef = half4(4.5,4.5,4.5,1.0);
half3 _EllipsoidCenter = half3(0,5.5,0);
half3 _FarShadow = half3(125,1,1);

sampler2D _WindNoise;
half4 _WindWeights = half4(0.5,0.5,0.5,0.5);
half4 _MmDd = half4(0.1,0.1,0.1,0.1);
float3 _Wind_time = float3(0,0,0);
half _CoreSize = 0.2;

half4 treeWind(half4 MmDd, half3 hash,float3 t){
    half2 uvd=half2(0.028f,0.056f);
    half2 uv=uvd*t.x+hash.xz*0.002f;
    half noise = tex2Dlod(_WindNoise,half4(uv,0,0)).r;
    uv=-uvd*(t.z + noise)+hash.xz*0.002f;
    half3 noise2=tex2Dlod(_WindNoise,half4(uv,0,0)).rgb*2;
    half dirNoise=(noise2.x-1)*MmDd.w;
    half magMod=cos(dirNoise*0.5f)*0.5f+0.5f;
    half magNoise=noise2.y*MmDd.y;
    return half4(sin(MmDd.z+dirNoise), 0, cos(MmDd.z+dirNoise),(MmDd.x+magNoise)*magMod);
}

half3 treeWindSway(half4 MmDd, half3 obj, half3 hash, half3 e2c, half3 e2, half4 weights, half coreSize, float3 t, half isLeaf)
{
    half4 windMain  = treeWind(MmDd, hash, t);
    
    float3 e1c  = float3(e2c.x,0,e2c.z);
    float3 e1    = float3(e2.x,e2c.y+e2.y,e2.z);
    float3 obje1=(obj-e1c)/e1;
    float3 obje2=(obj-e2c)/e2;
    float3 obje2n=normalize(obje2);
    float e2lb   =e2c.y-e2.y;
    float maincorr=dot(normalize(obj.xz-e2c.xz),windMain.xz);
    float mainmag=MmDd.x+MmDd.y;
    float4 threshold=float4(0.4,-0.0,0.2,0.5);
    weights=max((mainmag+1)*windMain.w*weights-threshold,(0).xxxx);
    float ifm=max(0,length(obje1)-coreSize)*min(obj.y/e2lb,1);
    float ifl=            ifm*weights.x*(-maincorr*0.5+0.5);
    float iff=isLeaf*ifm*weights.y*length(obje2);
    float ift=            ifm*weights.z*(maincorr*0.5+1);
    float ifb=                  weights.w*pow(obj.y/e1.y,2)*(-maincorr*0.125+1);
    float2 proj=float2(acos(obj.x/length(obj.xz)),normalize(obj).y);
    float2 uv=proj*0.03+hash.xy*0+t.yy*0.05;
    float3 tnoise=tex2Dlod(_WindNoise,half4(uv,0,0)).rgb-(0.2).xxx;
    uv=proj*0.2+hash.xy*0+t.yy*0.1;
    float3 fnoise=tex2Dlod(_WindNoise,half4(uv,0,0)).rgb-(0.2).xxx;
    float3 tside=cross(obje2n,float3(0,1,0));
    float3 tup  =cross(obje2n,-tside);
    float3 turb=tup*tnoise.y+tside*tnoise.x;
    return ifl*(float3(0,1,0)+windMain.xyz) + iff*fnoise + ift*turb + ifb*windMain.xyz;
}

half ellipsoidDepth(half3 ex, half3 n, half3 l)
{
  half invLen=length(ex);
  n/=ex;
  half ln=dot(l,n);
  half d=  dot(ln,ln)-dot(n,n)+1;
  half halfSqrtD=sqrt(max(d, 0));
  return max(halfSqrtD-ln, 0)*invLen;
}

half treeFarShadow(float3 posWS, float3 cameraPosWS, half3 ellipsoidPos, half3 ldir, half2 farShadowConf)
{
    half mix = saturate((distance(posWS, cameraPosWS)-farShadowConf.x)*0.1);
    return lerp(1, max(0,dot(ldir.xyz,normalize(ellipsoidPos))), mix*farShadowConf.y);
}

half treeOcclusionDepth(half4 ellipsoidDef, half3 ellipsoidPos, half3 ldir)
{   
#if defined(TREE_BASE_A)
    return max(0.5, ellipsoidDepth(ellipsoidDef.xyz,ellipsoidPos.xyz,-ldir)*ellipsoidDef.w);
#else
    return ellipsoidDepth(ellipsoidDef.xyz,ellipsoidPos.xyz,ldir)*ellipsoidDef.w;
#endif
}

half treeDiffuse(half initDiff, half d, half fs, half atten)
{
#if defined(TREE_BASE_B)
    initDiff = lerp(initDiff, 1,0.5) / (1+max(0,d-1)*atten); 
#elif defined(TREE_BASE_A)
    initDiff = lerp(initDiff, d, 0.5);
#else
    initDiff = 1;
#endif
    return initDiff*fs;
}

// Tranforms position from world to homogenous space
inline float4 treeWorldToClipPos(in float3 posWorld)
{
#if defined(STEREO_CUBEMAP_RENDER_ON)
    float3 offset = ODSOffset(posWorld, unity_HalfStereoSeparation.x);
    return mul(UNITY_MATRIX_VP, float4(posWorld + offset, 1.0));
#else
    return mul(UNITY_MATRIX_VP, float4(posWorld, 1.0));
#endif
}

#ifdef TREE_BASE_LEGACY

void vertTree(inout v2f o, inout float4 vertex, in float3 posWorld, in float2 uv)
{
    float3 ldir        = UnityWorldSpaceLightDir(posWorld);
    half3 windMovement = treeWindSway(_MmDd, posWorld-unity_ObjectToWorld._m03_m13_m23, unity_ObjectToWorld._m03_m13_m23, _EllipsoidCenter, _EllipsoidDef.xyz, _WindWeights, _CoreSize, _Wind_time, 1);
    
    half3 ellipsoidPos = posWorld -unity_ObjectToWorld._m03_m13_m23 -_EllipsoidCenter;
#if defined(TREE_BASE_B)
    half depth         = ellipsoidDepth(_EllipsoidDef.xyz,ellipsoidPos.xyz,ldir.xyz)*_EllipsoidDef.w;
#elif defined(TREE_BASE_A)
    half depth         = ellipsoidDepth(_EllipsoidDef.xyz,ellipsoidPos.xyz,-ldir.xyz)*_EllipsoidDef.w;
    depth              = max(0.5, depth);
#endif
    half farShadow     = treeFarShadow(posWorld, _WorldSpaceCameraPos, ellipsoidPos, ldir.xyz, _FarShadow);
    o.uv_d_fs          = float4(uv, depth, farShadow);
    o.pos              = treeWorldToClipPos(posWorld + float3(windMovement));
}
#endif //TREE_BASE_LEGACY

#endif //TREE_BASE_INCLUDED
