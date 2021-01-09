Shader "PostEffect/ColorWorld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		//_RedWorld("RedWorld",Range(0,1))=0.0
		//_GreenWorld("GreenWorld",Range(0,1)) = 0.0
		//_BlueWorld("BlueWorld",Range(0,1)) = 0.0
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
			//float _RedWorld;
			//float _GreenWorld;
			//float _BlueWorld;

			float3x3 rgbTolms;
			float3x3 lmsTorgb;
			/*float3x3 p_sm;*/
			float3x3 d_sl;
			/*float3x3 t_ml; */
			//float1x3 RGB;
			float3 LMS;
			float3 LMSp;
			float3 RGB;



            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
				float3x3 rgbTolms = {
					{0.31394,0.70760,0.04652},
					{0.15530,0.75796,0.08673},
					{0.01772,0.10945,0.87277},
				};

				float3x3 lmsTorgb = {
					{5.47213,-4.64189,0.16958},
					{-1.12464,2.29255,-0.16786},
					{0.02993,-0.19325,1.16339},
				};

				/*float3x3 p_sm = {
					{0,1.20800,-0.20797},
					{0,1,0},
					{0,0,1},
				};*/

				float3x3 d_sl = {
					{1,0,0},
					{0.82781,0,0.17216},
					{0,0,1},
				};

				/*
				float3x3 t_ml = {
					{1,0,0},
					{0,1,0},
					{-0.52543,1.52540,0},
				
				}; */
			

			//float1x3 RGB = {
				//{col.r,col.g,col.b},
			//};



				/*for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						answeRx1x3[i] += matrix3x3[i, j] * matrix1x3[j];
					}
				}*/

                float3 LMS = float3(mul(rgbTolms,float3(col.r,col.g,col.b)));
				float3 LMSp = float3(mul(d_sl,LMS));
				float3 RGB = float3(mul(lmsTorgb, LMSp));

				col.rgb = RGB;

                return col;
            }
            ENDCG
        }
    }
}
