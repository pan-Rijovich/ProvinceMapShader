Shader "Custom/BorderShower"
{
    Properties
    {
        _ProvinceTex ("Province", 2D) = "white" {}

        _RemapTex ("Remap", 2D) = "white" {}

        _PaletteTex ("Pallete", 2D) = "white" {}
        _TerrainTex ("Terrain", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "bump" {}

        _BorderSize ("Border Size", Range(0.00001, 0.001)) = 0.001
        _BorderColor ("Border Color", Color) = (0,0,0,1)
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

        float _BorderSize;
        fixed4 _BorderColor;

        struct Input
        {
            float2 uv_ProvinceTex;
            float2 uv_NormalMap;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

            // sample the texture
            fixed4 col = tex2D(_ProvinceTex, IN.uv_ProvinceTex);
                
            //return tex2D(_MainTex, i.uv) - tex2D(_MainTex, i.uv + float2(0.001, 0));
            fixed4 c1 = tex2D(_ProvinceTex, IN.uv_ProvinceTex + float2(_BorderSize, 0));
            fixed4 c2 = tex2D(_ProvinceTex, IN.uv_ProvinceTex - float2(_BorderSize, 0));
            fixed4 c3 = tex2D(_ProvinceTex, IN.uv_ProvinceTex + float2(0, _BorderSize));
            fixed4 c4 = tex2D(_ProvinceTex, IN.uv_ProvinceTex - float2(0, _BorderSize));

            if((any(c1 != col) || any(c2 != col) || any(c3 != col) || any(c4 != col))){
                fixed3 smoothness = (abs(col - c1) + abs(col - c2) + abs(col - c3) + abs(col - c4));
                o.Albedo = lerp(tex2D(_TerrainTex, IN.uv_ProvinceTex), _BorderColor, _BorderColor.a);
                return;
            }
            fixed4 index = tex2D(_RemapTex, IN.uv_ProvinceTex);
            fixed4 paletteValue = tex2D(_PaletteTex, index.xy * 255.0 / 256.0 + float2(0.001953125, 0.001953125));
                
            o.Albedo = lerp(tex2D(_TerrainTex, IN.uv_ProvinceTex), paletteValue, paletteValue.a);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
