Shader "Universal Render Pipeline/Sample"
{
	Properties
	{
		_MainTex("Texture",2D) = "white" {}
		_Scale("Scale",Float) = 20
	}
	SubShader
	{
		Cull Off 
		ZWrite Off
		ZTest Always

		Pass
		{
			Name "Sample"

			HLSLPROGRAM


			#pragma vertex vert
			#pragma fragment frag

			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "UnityCG.cginc"

			CBUFFER_START(UnityPerMaterial)
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform half _Scale;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				output.positionCS = UnityObjectToClipPos(input.positionOS);
				output.uv = ComputeGrabScreenPos(output.positionCS);
				//VertexPositionInputs vertex = GetVertexPositionInputs(input.positionOS.xy);
				//output.positionCS = vertex.positionCS;
				//output.uv = TRANSFORM_TEX(_MainTex,input.uv);
				return output;
			}

			float4 frag(Varyings input) : SV_Target
			{
				float2 scale = _MainTex_TexelSize.xy * _Scale;
				float2 uv = (floor(input.uv / scale) + 0.5)*scale;

				float4 color = tex2D(_MainTex, uv);
				return color;
			}
			ENDHLSL
		}
	}
}