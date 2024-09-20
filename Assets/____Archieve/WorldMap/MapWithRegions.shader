Shader "Custom/RegionShaderMultiPlayer"
{
    Properties
    {
        _RegionTex ("Region Texture", 2D) = "white" {}
        _PlayerColors ("Player Colors", Color) = (1,1,1,1)
        _PlayerIDs ("Player IDs", Float) = 0.0 
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderWidth ("Border Width", Float) = 1.0
        _PlayerCount ("Player Count", Int) = 1
        _SmoothFactor ("Smooth Factor", Float) = 1.0
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

            #include "UnityCG.cginc"

            sampler2D _RegionTex;
            float4 _PlayerColors[10];
            float _PlayerIDs[10];
            float4 _BorderColor;
            float _BorderWidth;
            int _PlayerCount;
            float _SmoothFactor;

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

            float GetDistance(float2 uv)
            {
                float2 offset[4] = { float2(1,0), float2(-1,0), float2(0,1), float2(0,-1) };
                float4 currentColor = tex2D(_RegionTex, uv);
                float minDistance = 1.0;

                for (int i = 0; i < 4; i++)
                {
                    float2 neighborUV = uv + offset[i] * _BorderWidth / _ScreenParams.xy;
                    float4 neighborColor = tex2D(_RegionTex, neighborUV);

                    if (currentColor.r != neighborColor.r || currentColor.g != neighborColor.g || currentColor.b != neighborColor.b)
                    {
                        float distance = length(offset[i] * _BorderWidth / _ScreenParams.xy);
                        minDistance = min(minDistance, distance);
                    }
                }

                return minDistance;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 regionColor = tex2D(_RegionTex, i.uv);
                float distance = GetDistance(i.uv);

                float borderAlpha = smoothstep(0.0, _SmoothFactor * _BorderWidth / _ScreenParams.x, distance);

                int regionID = int(regionColor.r * 255);

                for (int j = 0; j < _PlayerCount; j++)
                {
                    if (regionID == int(_PlayerIDs[j]))
                    {
                        float4 playerColor = _PlayerColors[j];
                        return lerp(_BorderColor, playerColor, borderAlpha);
                    }
                }

                return lerp(_BorderColor, regionColor, borderAlpha);
            }
            ENDCG
        }
    }
}
