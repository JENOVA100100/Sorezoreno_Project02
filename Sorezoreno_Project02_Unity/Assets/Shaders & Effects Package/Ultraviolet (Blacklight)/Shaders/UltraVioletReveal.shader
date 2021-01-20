Shader "Custom/UltraVioletReveal" {
	//OneGuyCoding made me! Thanks for buying me!
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "blank" {}
		_BumpAmount("Bumpmap Strength", float) = 1
		
		_LightColor ("LightColor", Color) = (0.8,0,1, 1)
	}
	SubShader
	{
		Name "StandardAntiLight"
		Tags 
		{
			"Queue"="Transparent" 
			"RenderType"="Fade"
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
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float _BumpAmount;
		float4 _Color;
		float4 _LightColor;

		half4 LightingSpecular (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) 
		{
			half4 c;
			c.rgb = s.Albedo.rgb;
			
			c.a = 0;
			if (distance(_LightColor0.rgb * (atten * 2), _LightColor * (atten * 2)) < 0.4 * (atten * 2))
			{
				c.a = s.Alpha * (atten * 2);
			}

			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutputCustom o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Normal = UnpackScaleNormal(tex2D (_BumpMap, IN.uv_BumpMap), _BumpAmount);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
