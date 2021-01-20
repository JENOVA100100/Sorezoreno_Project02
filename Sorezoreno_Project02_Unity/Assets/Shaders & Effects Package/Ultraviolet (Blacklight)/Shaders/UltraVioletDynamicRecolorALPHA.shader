Shader "Custom/UltraVioletDynamicRecolorALPHA" 
{
	//OneGuyCoding made me! Thanks for buying me!
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "blank" {}
		_BumpAmount("Bumpmap Strength", float) = 1
		//_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_EmissionAmount("Emission Amount", float) = 0
		_Shininess("Gloss Amount", float) = 0
		_Cube ("Cubemap", CUBE) = "" {}
		_ReflectAmount("Reflect Amount", float) = 0
		_LightColor ("LightColor", Color) = (0.8,0,1, 1)
		_ColorCount ("Color Count", int) = 8
		_Precision ("Precision", Range(0, 1)) = 0.1
	}
	SubShader
	{
		Name "StandardAntiLight"
		Tags 
		{
			"Queue"="Geometry" 
			"RenderType"="Opaque"
		}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Specular alpha
		#pragma target 3.0
		
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			fixed3 ScreenPos;
			fixed3 SkyRGB;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		samplerCUBE _Cube;
		float _BumpAmount;
		float _Shininess;
		float4 _Color;
		float4 _LightColor;
		//float4 _EmissionColor;
		float _EmissionAmount;
		float _Precision;
		float _ReflectAmount;

		int _ColorCount;
		uniform float4 _ColorArray[8];
		uniform float4 _ColorReplaceArray[8];
		uniform float _PrecisionArray[8];

		half4 LightingSpecular (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) 
		{
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0) * _Shininess;
			half4 c;
			c.rgb = s.Albedo.rgb;
			c.rgb = lerp(s.Albedo.rgb, s.SkyRGB * (_ReflectAmount * 5), _ReflectAmount);
			c.rgb *= _Color.rgb;
			c.a = s.Alpha;
			
			if (distance(_LightColor0.rgb * (atten * 2), _LightColor * (atten * 2)) < _Precision * (atten * 2))
			{
				for (int i = 0; i < _ColorCount; i++)
				{
					if (distance(s.Albedo.rgb, _ColorArray[i]) < _PrecisionArray[i])
					{
						c.rgb = (_ColorReplaceArray[i] * _EmissionAmount * diff) * (atten * 2);
						return c;
					}
				}
				c.rgb = (c.rgb * 0.5 * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
				return c;
			}
			c.rgb = (c.rgb * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 Grabpos : TEXCOORD0;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputCustom o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackScaleNormal(tex2D (_BumpMap, IN.uv_BumpMap), _BumpAmount);
			o.Albedo = c.rgb;
			o.Alpha = c.a * _Color.a;
			o.Gloss = _Shininess;
			float3 skyrgb = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb;
			o.SkyRGB = skyrgb;
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/Diffuse"
}