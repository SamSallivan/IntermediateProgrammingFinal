/*********************************************************************************
2017-10-19 11:55:57
@yvanliao
1.ͨ�� Image ������ color.r ���Ʊ��Ͷ�
2.֧��ETC1 ��Ҳ�����Լ�����͸����ͼ��
3.֧��UGUI Mask


2017-11-02 22:01:30
@yvanliao
1.ͨ�� Image ������ color.g �������ȣ�ֵΪ0ʱȫ�ڣ�

2017-11-03 15:46:50
@yvanliao
1.������һ�����Ʊ��ͶȺ����ȵ�UI shader��ԭshader��ԭΪrgb = 0ʱ����

*********************************************************************************/


Shader "Unlit/Transparent Alpha Split Gray"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex("Sprite Alpha Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaFactor("SrcAlphaFactor()", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaFactor("DstAlphaFactor()", Float) = 10

        _BorderBlend("Border Blend Range", vector) = (0,0,0,0)
		_BorderBlendAlpha("Border Blend Alpha Range", vector) = (0,0,0,0)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		[Toggle(UNITY_USE_OWN_ALPHA)] _UseOwnAlpha("Use Own Alpha", Float) = 0
        
        _ClipRect("Clip Rect", Vector) = (0, 0, 10000, 10000)
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
		Blend SrcAlpha OneMinusSrcAlpha,[_SrcAlphaFactor][_DstAlphaFactor]
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
			#include "Assets/CGInclude/LGameSysUI.cginc"
	
			//͸���Ȳ��еĿ���
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_USE_OWN_ALPHA
			#pragma multi_compile __ USE_CURVE
			#pragma multi_compile __ USE_PARTMASK_RGBA

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
			half4 _ClipRect;
            half4 _BorderBlend;
			float4 _BorderBlendAlpha;

#ifdef USE_PARTMASK_RGBA 
			float4 _partMaskUV;
#endif

#ifdef USE_CURVE
			float _UI_CurveLength;
#endif

			inline float GetSoft2DClipping(in float2 position, in float4 clipRect, in float4 borderBlend)
			{
				//float2 range = max(0, float2(horizontal , vertical));
				float2 inside = smoothstep(clipRect.xy, clipRect.xy + borderBlend.xy, position.xy) * smoothstep(clipRect.zw, clipRect.zw - borderBlend.zw, position.xy);
				return inside.x * inside.y;
			}

			VS_OUTPUT vert (VS_INPUT IN)
			{
				VS_OUTPUT OUT;
				OUT.worldPosition = IN.vertex;
#ifdef USE_CURVE
				float4 pos = mul(UNITY_MATRIX_MV, IN.vertex);
				float rate = length(pos.x) / _UI_CurveLength;
				float w = pos.w + rate * length(rate);
				pos.yz /= w;
				OUT.pos = mul(UNITY_MATRIX_P, pos);
#else
				OUT.pos = UnityObjectToClipPos(OUT.worldPosition);
#endif
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			}

			fixed4 frag (VS_OUTPUT IN) : SV_Target
			{
#ifdef UNITY_USE_OWN_ALPHA 
				fixed4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
#else
	
	#ifdef USE_PARTMASK_RGBA 
				fixed4 baseColor = tex2D(_MainTex, IN.texcoord);
				fixed4 maskColor = tex2D(_AlphaTex, IN.texcoord);
	
				float maskStepU = step(_partMaskUV.x, IN.texcoord.x) * step(IN.texcoord.x, _partMaskUV.z);
				float maskStepV = step(_partMaskUV.y, IN.texcoord.y) * step(IN.texcoord.y, _partMaskUV.w);
				fixed4 color = lerp(baseColor, maskColor, maskStepU * maskStepV);
	#else
				//ʹ��unity�Դ��ĺ��������������͸��������
				fixed4 baseColor = tex2D(_MainTex, IN.texcoord);
				fixed4 alphaColor = tex2D(_AlphaTex, IN.texcoord);

				fixed4 color = fixed4(baseColor.rgb + _TextureSampleAdd.rgb, baseColor.a * alphaColor.r + _TextureSampleAdd.a);
	#endif
#endif
				
				//Mask��ص�һЩ����
                color.a *= LGameGetSoft2DClippingEx(IN.worldPosition, _ClipRect, _BorderBlend, _BorderBlendAlpha) * IN.color.a;


				//���Ͷȼ��㣬ʹ�ö���ɫ�� r ֵ���Ʊ��Ͷ�
				color.rgb = lerp( Luminance(color.rgb).rrr , color.rgb , IN.color.r);

				//���ȼ��㣬ʹ�ö���ɫ�� g ֵ��������
				color.rgb *= IN.color.g;

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
