Shader "Custom/MapShowerSimple"
{
    Properties
    {
        _ProvinceTex ("Province", 2D) = "white" {}

        _RemapTex ("Remap", 2D) = "white" {}

        _PaletteTex ("Pallete", 2D) = "white" {}
        _TerrainTex ("Terrain", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "bump" {}

        _BorderSize ("Border Size", Range(0.0, 0.001)) = 0.0001
        _BorderColor ("Border Color", Color) = (0,0,0,1)

        [Toggle] _BoolTest("is Bending", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _ProvinceTex;
        sampler2D _RemapTex;
        sampler2D _PaletteTex;
        sampler2D _TerrainTex;
        sampler2D _NormalMap;
        float4 _ProvinceTex_TexelSize;
        fixed4 counter = 0;

        float _BorderSize;
        fixed4 _BorderColor;
        bool _BoolTest;

        struct Input
        {
            float2 uv_ProvinceTex;
            float2 uv_NormalMap;
        };

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 get_secondary_color(float2 uv)
        {
            fixed4 index = tex2D(_RemapTex, uv);
            fixed4 paletteValue = tex2D(_PaletteTex, index.xy * 255.0 / 256.0 + float2(0.001953125, 0.001953125));
            return paletteValue;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_ProvinceTex;

            uv -= _ProvinceTex_TexelSize.xy * 0.5;

            float2 textureSize = float2(_ProvinceTex_TexelSize.z, _ProvinceTex_TexelSize.w);

            float2 pixelCoord = uv * textureSize;

            float2 bottomLeft = floor(pixelCoord);
            float2 topRight = bottomLeft + float2(1, 1);

            float2 uvBL = (bottomLeft + 0.5) / textureSize;
            float2 uvBR = (float2(topRight.x, bottomLeft.y) + 0.5) / textureSize;
            float2 uvTL = (float2(bottomLeft.x, topRight.y) + 0.5) / textureSize;
            float2 uvTR = (topRight + 0.5) / textureSize;


            fixed4 colorBL = get_secondary_color(uvBL);
            fixed4 colorBR = get_secondary_color(uvBR);
            fixed4 colorTL = get_secondary_color(uvTL);
            fixed4 colorTR = get_secondary_color(uvTR);

            float2 f = frac(pixelCoord);
            float wBL = (1.0 - f.x) * (1.0 - f.y);
            float wBR = f.x * (1.0 - f.y);
            float wTL = (1.0 - f.x) * f.y;
            float wTR = f.x * f.y;

            fixed4 finalColor = colorBL * wBL + colorBR * wBR + colorTL * wTL + colorTR * wTR;

            o.Albedo = lerp(tex2D(_TerrainTex, IN.uv_NormalMap), finalColor, finalColor.a).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
