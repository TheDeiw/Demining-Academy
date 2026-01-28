Low-power enviroment v1.0.3
Made with Unity 2019 and Unity 6.
================Features===============
This package delivers tree shaders with wind simulation and 
simulation of treetop occlusion/illumination as an ellipsoid with configurable flat density.
Additionally, translucency on SRP and URP shaders is implemented.
LOD cross-fading is supported, multiple lights are supported, fog is supported.
Lightmapping is only supported on HDRP.
The SRP and URP shaders use Unity terrain engine per-instance tree parameters of color and size.
Unity terrain engine billboarding is not supported, although there was an attempt
 - see commented-out lines Dependency "BillboardShader" =... at the ends of SRP shaders. 
Reason is that configuration of the billboard shader is hardcoded in the engine.

HDRP, URP, SRP, both legacy Unity 4 and standard Unity 5 shader, are supported.

Pipeline-dependent assets are in subpackages HDRP, URP, SRP, which need to be seelcted for import depending on your target pipelines.

Effect of ellipsoid shading and occlusion texture is dependent on realtime shadows strength and enviroment lighting intensity multiplier.

==================Usage=================
There is one shader for both bark and leaves.
Use 'Material Type' 'Standard' for HDRP bark, use 'Material Type' 'Translucent' for leaves.
Use cull mode 'Back'/'One sided' for bark and 'Off'/'Double-sided' for leaves.

The shaders are connected to editor tool 'LP'. 
Selecting this tool in the toolbar will visualize tree top ellipsoids on trees selected in Hierarchy.
Wind parameters are globally controlled by TreeWindParams.cs

==============Wind parameters===========
Wind parameters are set by TreeWindParams.cs. 
The script must be present in a scene for the wind simulation to run, 
because it also provides time of the wind simulation in the shaders.
The time calculation is hardcoded, but if you need to change the frequency of oscillation of animation
or its relation to magnitude, edit it in this script. 
The time is vector of 3. First component has constant speed, 
second is proportionate to magnitude and magnitude turbulence  and third one to magnitude turbulence only.

*Magnitude - main wind magnitude
*Magnitude turbulence - amplitude of magnitude oscilation.
*Direction - wind direction, <-π;π>
*Direction turbulence - amplitude of direction oscilation, range <-π;π>. Sign can be ignored.
*Transition time - smoothing of changes in parameters above. The time is in seconds.

=============Shader parameters==========
*Mask
HDRP shader only. R - unused, G - ambient occlusion, B - thickness, A - smoothness.
For URP and SRP, use Occlusion.

*MaskIntensity
HDRP shader only. Multiplier of Mask channels. Values outside 0-1 range are allowed.

*Leaves bound extent and density / Ellipsoid definition / EllipsoidDef
Approximation of tree top. Should wrap the tree top as cloesly as possible.
First three values are ellipsoid extents X,Y,Z, the fourth one is ellipsoid density.

*Leaves bound position / Ellipsoid center / EllipsoidCenter
Center of the ellipsoid.
Setting center Y equal to extent Y will cause zero-division in vertex and fragment shaders,
causing the whole mesh to disappear on some platforms/pipelines.
To avoid mismatch of wind simulation on individual materials, 
all materials on a single mesh should have the same ellipsoid extents and center.

*Far shadow fade / Far shadow / FarShadeDef
Adds shading of the tree top once outside of range of casted shadows. 
The range is set by the X coordinate, strength by Y coordinate.

*Wind noise / Perlin
Source of randomness for the wind simulation. Only R ang G channels are used.

*Wind weights (LFTB)
Weight of individual components of the wind simulation - lift, flutter, turbulence and bend.
Negative values are allowed.

*Core size
Size of the core part of ellipsoid, which should not be affected by wind. 
This will prevent thick branches in the core of the tree from looking too fluid.

===========Shader implementation========
HDRP is implemented using shader graph, all other shaders are plain HLSL.
The implementations of SRP and URP shaders are using modified implementations of Unity vertex, fragment and BRDF functions.
There are two approaches of simulation of tree top shading - dimming fragments most concealed from directional light, or brightening the most exposed ones.
HDRP and URP shaders use dimming, SRP shaders use brightening.
Simulation mode in SRP and URP can be switched by defining TREE_BASE_A for brightening or TREE_BASE_B for dimming.
The switches are defined in SRPStdOverride.cginc for SRP standard shader, TreeSRPLegacy.shader for SRP legacy shader and URPStdOverride.hlsl for URP shader.

If TREE_BASE_B is used, shadow strength must be less than one for the effect to be visible.




