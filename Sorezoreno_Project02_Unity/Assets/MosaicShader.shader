Shader "PostEffect/MosaicShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 20 // この変数の値をC#でいじりたい
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

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform half _Scale;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            

            fixed4 frag (v2f i) : SV_Target
            {
				float2 scale = _MainTex_TexelSize.xy * _Scale;
				float2 uv = (floor(i.uv / scale) + 0.5) + scale;


                fixed4 col = tex2D(_MainTex, uv);
               
                return col;
            }
            ENDCG
        }
    }
}
