Shader "PostEffect/WorldMonotone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		
		_GrayColorR ("GrayColorRed",Range(0,1)) = 0.0
		_GrayColorG("GrayColorGreen",Range(0,1)) = 0.0
		_GrayColorB("GrayColorBlue",Range(0,1)) = 0.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			float _GrayColorR;
			float _GrayColorG;
			float _GrayColorB;

            fixed4 frag (v2f i) : SV_Target
			{

				float4 col = tex2D(_MainTex, i.uv);
				float3 grayVec = float3(0.2126,0.7152,0.0722);
				float g = dot(col.rgb, grayVec);
				//col = fixed4(g, g, g, 1);
				col.rgb = float3(g+_GrayColorR, g+_GrayColorG, g+_GrayColorB);
				return col;
			}
			ENDCG
        }
    }
}
