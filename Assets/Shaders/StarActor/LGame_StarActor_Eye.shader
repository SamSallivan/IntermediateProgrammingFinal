﻿Shader "LGame/StarActor/Eye"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_IrisMap("Iris", 2D) = "white" {}
		_MainTex("Albedo", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Scale", Range(0.0,1.0)) = 1.0
		_OcclusionMap("Occlusion Map",2D) = "white"{}
		_OcclusionStrength("Occlusion Strength",Range(0,1)) = 1
		[Enum(uv0,0,uv1,1)] _OcclusionUVChannel("Occlusion texture UV", Float) = 0
		[Enum(MatCap,0,CubeMap,1)] _ReflectionType("Reflection Type", Float) = 0
		[HDR]_ReflectionColor("Reflection Color", Color) = (0.5 , 0.5 , 0.5 , 0)
		_ReflectionMatCap("Reflection Texture", 2D) = "" {}
		_ReflectionCubeMap("Reflection Texture", Cube) = "" {}
		_AmbientCol("Ambient Color" , Color) = (0.5,0.5,0.5,0.5)
		_ShadowStrength("Self Shadow Strength" , Range(0,1)) = 1
		[HDR]_RakingLightColor("Raking Light Color" , Color) = (0,0,0,0)
		_RakingLightSoftness("Raking Light Softness",Float) = 4.0
		_BrightnessInOcclusion("Brightness In Occlusion" , Range(0,1)) = 0.5
		_BrightnessInShadow("Brightness In Shadow" , Range(0,1)) = 0.5
		_DirLight("Dir Light" , Vector) = (-1,0,0,0)
		[Enum(Simple,0,Refraction,1)] _EyeType("Eye Type", Float) = 0
		_EyeDataMap("Iris Mask/Depth",2D) = "white"{}
		_IrisNormalMap("Iris Normal Map",2D) = "bump"{}
		_RefractionStrength("Refraction Strength",Float) = 1.0
		_Radius("Radius",Range(0.0,1.0)) = 0.216
		_IOR("IOR",Range(0.0,1.0)) = 0.568
		_FrontDir("Front Dir",Vector) = (0,0,1,0)
		_RightDir("Right Dir",Vector) = (-1,0,0,0)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "AlphaTest" "PerformanceChecks" = "False" }
		LOD 300
		Pass
		{
			Stencil {
				Ref 16
				Comp always
				Pass replace
			}
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }
			ZWrite On
			CGPROGRAM
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight noshadowmask  
			//#pragma multi_compile __ _USE_DIRECT_GPU_SKINNING
			#pragma skip_variants POINT_COOKIE LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH
			#pragma vertex vert_base
			#pragma fragment frag_base
			#pragma target 3.0
			#pragma multi_compile _ _HARD_SHADOW _SOFT_SHADOW
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _OCCLUSION_UV1
			#pragma shader_feature _EYE_REFRACTION
			#pragma shader_feature _REFLECTION_CUBEMAP
			#define _EYE
			#include "Assets/CGInclude/LGameStarActorCG.cginc"
			ENDCG
		}
		Pass
		{
			Name "FORWARD_DELTA"
			Tags{ "LightMode" = "ForwardAdd" }
			Blend One One
			ZWrite Off
			ZTest LEqual
			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_fwdadd  
			//#pragma multi_compile __ _USE_DIRECT_GPU_SKINNING
			#pragma skip_variants DIRECTIONAL_COOKIE POINT_COOKIE LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH SHADOWS_SCREEN SPOT
			#pragma vertex vert_add
			#pragma fragment frag_add
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _OCCLUSION_UV1
			#pragma shader_feature _EYE_REFRACTION
			#define _EYE
			#include "Assets/CGInclude/LGameStarActorCG.cginc"
			ENDCG
		}
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On 
			ZTest LEqual
			CGPROGRAM
			//#pragma multi_compile __ _USE_DIRECT_GPU_SKINNING
			#pragma vertex Vert_Shadow
			#pragma fragment Frag_Shadow
			#pragma multi_compile_shadowcaster
			#pragma skip_variants SHADOWS_CUBE LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH SHADOWS_SCREEN
			#define _EYE
			#include "Assets/CGInclude/LGameStarActorShadowCaster.cginc"	
			ENDCG
		}
	}
	CustomEditor "LGameSDK.AnimTool.LGameStarActorEyeShaderGUI"
}
