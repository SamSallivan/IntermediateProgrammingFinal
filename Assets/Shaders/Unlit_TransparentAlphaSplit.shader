/*********************************************************************************
2017-10-19 11:55:57
@yvanliao
1.ͨ�� Image ������ color.r ���Ʊ��Ͷ�
2.֧��ETC1 ��Ҳ�����Լ�����͸����ͼ��
3.֧��UGUI Mask


2017-11-02 22:01:30
@yvanliao
1.ͨ�� Image ������ color.g �������ȣ�ֵΪ0ʱȫ�ڣ�


2017-11-03 15:48:02
@yvanliao
1.rgbֵΪ0ʱ����ɫ��Ҷ�ͼ

*********************************************************************************/


Shader "Unlit/Transparent Alpha Split"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex("Sprite Alpha Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	
	SubShader
	{
		//����unityģ���޸���tag
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
		Pass
		{
			Name "Default"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			//ò��û�õ�����꣬ע�͵��ˡ�	by��yvanliao
           // #pragma multi_compile GRAY_OFF GRAY_ON
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
	
			//͸���Ȳ��еĿ���
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct VS_INPUT
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
	
			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 _Color;

			fixed4 _TextureSampleAdd;
			float4 _ClipRect;


			VS_OUTPUT vert (VS_INPUT IN)
			{
				VS_OUTPUT OUT;
				OUT.worldPosition = IN.vertex;
				OUT.pos = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			}

			fixed4 frag (VS_OUTPUT IN) : SV_Target
			{

				//ʹ��unity�Դ��ĺ��������������͸��������
				fixed4 color = UnityGetUIDiffuseColor(IN.texcoord, _MainTex, _AlphaTex, _TextureSampleAdd) ;
				
				//Rect Mask��ص�һЩ���� ��������Կ������������Soft Mask��
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect) * IN.color.a;


				//���Ͷȼ��㣬���color��rgb��Ϊ0�ͱ�ɺڰ�ͼ
				color.rgb = lerp( Luminance(color.rgb).rrr , color.rgb * IN.color.rgb, step(0.01,IN.color.r + IN.color.g + IN.color.b ));

				//ʹ��͸���Ȳ���
				#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
				#endif

				return color;
			}
			ENDCG
		}
	}
}
