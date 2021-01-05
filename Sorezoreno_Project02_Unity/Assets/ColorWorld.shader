Shader "PostEffect/ColorWorld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RedWorld("RedWorld",Range(0,1))=0.0
		_GreenWorld("GreenWorld",Range(0,1)) = 0.0
		_BlueWorld("BlueWorld",Range(0,1)) = 0.0
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
			float _RedWorld;
			float _GreenWorld;
			float _BlueWorld;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                col.rgb = float3(col.r+_RedWorld,col.g+_GreenWorld,col.b+_BlueWorld);
                return col;
            }
            ENDCG
        }
    }
}
