Shader "Custom/MapShowerSimple"
{
    Properties
    {
        _ProvinceTex ("Province", 2D) = "white" {}

        _RemapTex ("Remap", 2D) = "white" {}

        _PaletteTex ("Pallete", 2D) = "white" {}
        _TerrainTex ("Terrain", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "bump" {}
        _BorderTexture ("Border", 2D) = "black" {}
        _BorderColorPalette ("BorderColor", 2D) = "black" {}
        _BorderColor ("BorderColor", Color) = (1,1,1,1)
        _Tilling ("Tilling", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma multi_compile_instancing
        #pragma target 3.0

        sampler2D _ProvinceTex;
        sampler2D _RemapTex;
        sampler2D _PaletteTex;
        sampler2D _TerrainTex;
        sampler2D _NormalMap;
        sampler2D _BorderTexture;
        sampler2D _BorderColorPalette;
        float4 _BorderColor;
        float4 _RemapTex_TexelSize;
        fixed4 counter = 0;

        //UNITY_DECLARE_TEX2D(_BorderTexture);

        struct Input
        {
            float2 uv_RemapTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Tilling)
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 get_secondary_color(float2 uv)
        {
            fixed4 index = tex2D(_RemapTex, uv);
            fixed4 paletteValue = tex2D(_PaletteTex, index.xy * 255.0 / 256.0 + float2(0.001953125, 0.001953125));
            return paletteValue;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 tilling = UNITY_ACCESS_INSTANCED_PROP(Props, _Tilling);
            float2 chunkUV =  IN.uv_RemapTex * tilling.xy + tilling.zw;

            float2 uv = chunkUV - _RemapTex_TexelSize.xy * 0.5;

            float2 textureSize = float2(_RemapTex_TexelSize.z, _RemapTex_TexelSize.w);

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

            
            
            vector<float, 4> borderValue = tex2D(_BorderTexture, IN.uv_RemapTex);

            float borderPalettePixStep = 1.0 / 512.0;
            float2 borderPaletteUV = IN.uv_RemapTex;
            borderPaletteUV.y = 0.5f;
            borderPaletteUV.x = lerp(borderPalettePixStep, 1.0-borderPalettePixStep, borderValue.x);
            
            float4 borderColor = tex2D(_BorderColorPalette, borderPaletteUV);
            finalColor = lerp(finalColor, _BorderColor, borderValue.x);

            o.Albedo = lerp(tex2D(_TerrainTex, chunkUV), finalColor, finalColor.a).rgb;

            fixed4 normalColor = tex2D(_NormalMap, chunkUV);
            o.Normal = UnpackNormal(normalColor);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
