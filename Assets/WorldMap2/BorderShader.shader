Shader "Custom/BorderShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Основная текстура
        _Distortion ("Distortion", Float) = 0.05 // Сила искажения
        _Frequency ("Frequency", Float) = 10.0 // Частота искажения
        _Speed ("Speed", Float) = 1.0 // Скорость искажения
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

            sampler2D _MainTex;
            float _Distortion;
            float _Frequency;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Деформация текстурных координат
                float wave = sin(i.uv.x * _Frequency + _Time * _Speed) * _Distortion;
                float2 distortedUV = float2(i.uv.x, i.uv.y + wave);

                // Получение цвета из текстуры
                float4 texColor = tex2D(_MainTex, distortedUV);

                return texColor;
            }
            ENDCG
        }
    }
}