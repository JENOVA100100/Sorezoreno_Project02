Shader "PostEffect/MosaicRyu"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Width("Width",Int) = 160
		_Height("Height",Int) = 90
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
			float _Width;
			float _Height;

            fixed4 frag (v2f i) : SV_Target
            {
				float2 grid;
				grid.x = floor(i.uv.x * _Width) / _Width;
				grid.y = floor(i.uv.y * _Height) / _Height;

                fixed4 col = tex2D(_MainTex, grid);
               
                return col;
            }
            ENDCG
        }
    }
}
