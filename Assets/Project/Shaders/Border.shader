Shader "Custom/TransparentTextureShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Основна текстура
        _Color ("Color Tint", Color) = (1,1,1,1) // Колір з можливістю налаштування альфа-каналу
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            // Включаємо альфа-блендінг для підтримки прозорості
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Вхідні дані
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Вихідні дані
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // Шейдерні властивості
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            // Вершинний шейдер
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Фрагментний шейдер
            fixed4 frag (v2f i) : SV_Target
            {
                // Отримання кольору текстури
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // Множимо на _Color для можливості змінювати колір та прозорість
                return texColor * _Color;
            }
            ENDCG
        }
    }
}
