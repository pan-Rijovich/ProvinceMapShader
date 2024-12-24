Shader "Custom/SmoothTest"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BorderSize ("Border Size", Range(0.0, 0.001)) = 0.0001
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

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _BorderSize;
        float4 _MainTex_TexelSize; // ������ ������ ������� �������� (������������� ������� Unity)


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;

            // ������ ��������
            float2 textureSize = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);

            // ����������� UV � ���������� �������� ��������
            float2 pixelCoord = uv * textureSize;

            // ���������� ������� ��������� ��������
            float2 bottomLeft = floor(pixelCoord); // ����� ������ �������
            float2 topRight = bottomLeft + float2(1, 1); // ������ ������� �������

            // ��������� UV ������� �� ��������� 4-� ��������
            float2 uvBL = (bottomLeft + 0.5) / textureSize; // ����� ������
            float2 uvBR = (float2(topRight.x, bottomLeft.y) + 0.5) / textureSize; // ������ ������
            float2 uvTL = (float2(bottomLeft.x, topRight.y) + 0.5) / textureSize; // ����� �������
            float2 uvTR = (topRight + 0.5) / textureSize; // ������ �������

            // ��������� ����� ��������� 4-� ��������
            fixed4 colorBL = tex2D(_MainTex, uvBL);
            fixed4 colorBR = tex2D(_MainTex, uvBR);
            fixed4 colorTL = tex2D(_MainTex, uvTL);
            fixed4 colorTR = tex2D(_MainTex, uvTR);

            // ��������� ���������� �� ������� UV �� ������� �������
            float2 f = frac(pixelCoord); // ������� ����� - ���������� �� ������� ������ ����
            float wBL = (1.0 - f.x) * (1.0 - f.y); // ��� ������ �������
            float wBR = f.x * (1.0 - f.y);         // ��� ������� �������
            float wTL = (1.0 - f.x) * f.y;         // ��� ������ ��������
            float wTR = f.x * f.y;                 // ��� ������� ��������

            // �������� ���� � ������ �����
            fixed4 finalColor = colorBL * wBL + colorBR * wBR + colorTL * wTL + colorTR * wTR;

            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
