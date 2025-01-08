Shader "Custom/VertexColorToAlbedo"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        struct Input
        {
            float4 color : COLOR; // Колір вершини
        };

        float _Glossiness;
        float _Metallic;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Використовуємо колір вершини для альбедо
            o.Albedo = IN.color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
