﻿
Shader "LGame/Character/Player_GhostShadow"
{																		
	Properties
	{
		[HideInInspector] _OptionMode("__OptionMode",float) = 0	
		[HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[Enum(Off, 0, On, 1)]_ZWriteMode ("__ZWriteMode", float) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode ("__CullMode", float) = 2
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode ("__ZTestMode", Float) = 4
	
		_OffsetColor ("OffsetColor", Color) = (0,0,0,1)  //色彩偏移（受击闪白之类）
		_MainColor("Main Color" , Color) = (1,1,1,1)//染色	
		_MainTex ("Main Texture(RGBA)", 2D) = "white" {} //主纹理

		_SubColor("Sub Color" , Color) = (1,1,1,1)//染色	
		_SubTex("Sub Texture(RGBA)", 2D) = "white" {} //替换纹理
		_SubTexLerp("SubTexture Lerp", Range(0,1)) = 0 //主次纹理的插值
		[Enum(UV , 0, Screen , 1)] _SubTexMode("Sample Mode", Float) = 0.0

		_NoiseTex("Noise Texture(R)", 2D) = "white" {} //噪音纹理
		_NoiseTiling("Noise Tiling" , float) = 1
		[hdr]_EdgeColor("Edge Color" , Color) = (0,0,0,1) //溶解边缘颜色
		_EdgeWidth("Edge Width" , Range(0.01,0.5)) = 0.5 //溶解边缘宽度

		_MaskTex ("Mask (R for Metallic ,  B for Alpha , G for flowlight)", 2D) = "white" {} //遮罩贴图

		_MatCap("MatCap Texture (RGB)", 2D) = "" {} //MatCap贴图
		_MatCapColor("MatCap Color" , Color) = (1,1,1,1)//MatCap颜色
		_MatCapIntensity("MatCap Intensity", Range(0,8)) =1 //MatCap贴图强度

		[Enum(multiply, 0, Add , 1)] _RimLightBlendMode("RimlightBlendMode", int) = 0//边缘光混合模式
		_RimLightColor("RimLight Color" , Color) = (0,0,0,1) //边缘光颜色
		_RimLighRange("RimLigh Range", Range(0.1,10)) = 1 //边缘光范围
		_RimLighMultipliers ("RimLigh Multipliers", Range(0, 5)) = 0//边缘光强度

		[Enum(UV , 0, Screen , 1)] _FlowlightMode("Sample Mode", Float) = 0.0
		_FlowlightTex("Flowlight Texture" , 2D) = "" {} //自发光贴图
		_FlowlightCol("Flowlight Color", Color) = (0,0,0,1)  //自发光颜色
		_FlowlightMultipliers("Emission Multipliers", Float) =1 //自发光强度
		[Enum(multiply, 0, Add , 1,alpha,2)] _FlowLightBlendMode("BlendMode", int) = 1//流光混合模式		

		_DissolveTex("Dissolve Texture" , 2D) = "white" {} //溶解贴图
		_DissolveTilling("Dissolve Tilling" , float) = 1
		[hdr]_DissolveRangeCol("Range Color" , Color) = (0,0,0,0)
		_DissolveThreshold("Range Threshold" , Range(0,1)) = 1
		_DissolveRangeSize ("Range Size", range(0.01,0.5)) = 0.01

		_OutlineCol("OutlineCol", Color) = (0,0,0,1)
		_OutlineScale("Outline Scale", Range(0,2)) = 1
		_ScreenOutlineScale("Screen Outline Scale", Range(-1,2)) = 0
		_ScreenOutlineColor("Screen Outline Color", Color) = (0,0,0,1)

		_ShadowFalloff("Shadow Falloff" , Range(0.01,1)) = 0.5//阴影衰减

		_AlphaCtrl("AlphaCtrl", Range(0,1)) = 1

		[hdr]_GhostColor("Ghost Color" , Color) = (0,0,0,0)
		[hdr]_GhostEndColor("Ghost End Color" , Color) = (0,0,0,0)
		[IntRange]_GhostRimLightMultipliers ("RimLight Multipliers" , Range(1,10)) = 0
		_GhostVector("Ghost Vector(w for speed)" , Vector) = (0,0,0,0.1)
		_GhostVector2("Ghost Vector2(w for speed)" , Vector) = (0,0,0,0.1)
		[SimpleToggle]_MultiVector("Multi Vector" , int) = 0
		_GhostScale("Ghost Scale" , Range(0,2)) = 1
		_DepthOffset("Depth Offset", Range(-0.5,0.5)) = 0
		

	}

	//Base + Shadow + Outline																			   
	SubShader
	{
		Tags { "RenderType"="AlphaTest" "Queue"="AlphaTest" }
		LOD 75

		//default pass
		UsePass "Hidden/Character/Shadow/CharacterShadow"
		Pass
		{
			Name "CharacterDefault"
            Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWriteMode]
			ZTest [_ZTestMode]
			Cull [_CullMode]
			Lighting Off
			Fog { Mode Off }
			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile __ _SUBTEX 
			#pragma multi_compile __ _DISSOLVE _ALPHABLEND_ON 		
			#pragma multi_compile __ _METAL 			
			#pragma multi_compile __ _RIMLIGHT
			#pragma multi_compile __ _FLOWLIGHTUV _FLOWLIGHTSCREEN
			#pragma multi_compile __ _USE_DIRECT_GPU_SKINNING
			#pragma vertex vert
			#pragma fragment frag	
			#include "Assets/CGInclude/LGameCharacter.cginc"
			ENDCG
		}
		//UsePass "Hidden/Character/Outline/CharacterOutline" 

		UsePass "Hidden/Character/GhostShadow/BASE"
		UsePass "Hidden/Character/GhostShadow/BASE2"

		//srp pass
		UsePass "Hidden/Character/Shadow Srp/CharacterShadowSrp"
		UsePass "Hidden/Character/Shadow Srp/CharacterSoftShadowSrp"
		UsePass "Hidden/LGame/Character/PlayerDefault Srp/CharacterDefaultSrp"
		UsePass "Hidden/Character/Outline Srp/CharacterOutlineSrp"
		UsePass "Hidden/Character/Outline Srp/CharacterScreenOutlineSrp"
		UsePass "Hidden/Character/GhostShadow Srp/BaseSrp"
		UsePass "Hidden/Character/GhostShadow Srp/Base2Srp"
	}

	CustomEditor"LGameCharacterHeroGUI"
}
