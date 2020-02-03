Shader "Unlit/CalculateNDVI"
{
    Properties
    {
        _RGBTex ("RGB Image", 2D) = "white" {}
		_NIRTex("NIR Image", 2D) = "white" {}
		_ev_rgb("ev_rgb", float) = 0.0 
		_ev_nir("ev_nir", float) = 0.0
		_iso_rgb("iso_rgb", float) = 0.0 
		_iso_nir("iso_nir", float) = 0.0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _RGBTex, _NIRTex;
            float4 _RGBTex_ST, _NIRTex_ST;
			float _ev_rgb, _ev_nir, _iso_rgb, _iso_nir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _RGBTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 rgb = tex2D(_RGBTex, i.uv);
				float4 nir = tex2D(_NIRTex, i.uv);
				float BDS1 = -0.061 * rgb[1] - 0.182 * rgb[2] * 1.377 * rgb[3],
					  BDS2 = -0.329 * rgb[1] + 1.420 * rgb[2] - 0.199 * rgb[3],
					  BDS3 =  1.150 * rgb[1] - 0.110 * rgb[2] - 0.034 * rgb[3];
				float BDS4 = 1.000 * nir[1] - 0.956 * nir[3],
					  BDS5 = -0.341 * nir[1] + 2.436 * nir[3];
				float BDN1 = BDS1 / (_ev_rgb * (_iso_rgb / 100)),
					  BDN2 = BDS2 / (_ev_rgb * (_iso_rgb / 100)),
					  BDN3 = BDS3 / (_ev_rgb * (_iso_rgb / 100)),
					  BDN4 = BDS4 / (_ev_nir * (_iso_nir / 100)),
					  BDN5 = BDS5 / (_ev_nir * (_iso_nir / 100));
				float ndvi = (2.7 * BDN5 - BDN3) / (2.7 * BDN5 + BDN3);
				/**
				float RGBSEP1 = BDS3, RGBSEP2 = BDS2, RGBSEP3 = BDS1;
				float NIRSEP1 = BDS5, NIRSEP2 = BDS5, NIRSEP3 = BDS4;
				float RGBN1 = BDN3, RGBN2 = BDN2, RGBN3 = BDN1;
				float NIRN1 = BDN4, NIRN2 = BDN5, NIRN3 = BDN4;
				**/
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(0, ndvi, 0, 1);
            }
            ENDCG
        }
    }
}
