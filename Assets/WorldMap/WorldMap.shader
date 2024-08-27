Shader "Custom/WorldMap"
{
	Properties
    {
        _ProvinceMap ("Province Map", 2D) = "white" {} // ����� ���������
        _TerrainMap ("Terrain Map", 2D) = "white" {} // ����� ���������
        _BumpMap ("Normal Map", 2D) = "bump" {} // ���������� ����� ��� ������� �����������
        _Extrude ("Extrude", float) = 0.0 // ���� ��������� (������������) ��� �������������
        _DispMap ("Displacement Map", 2D) = "black" {} // ����� �������������

        // ����
        _RiverMaskMap ("RiverMask Map", 2D) = "white" {} // ����� ���
        _RiverTexture ("RiverMask Texture", 2D) = "white" {} // �������� ���

        // �������
        _TreeMaskMap ("TreeMask Map", 2D) = "white" {} // ����� ��������
        _PineTreeSprite ("PineTree Sprite", 2D) = "white" {} // ������ �������� ������
        _TreeSprite ("Tree Sprite", 2D) = "white" {} // ������ ������
        _PalmTreeSprite ("PalmTree Sprite", 2D) = "white" {} // ������ ���������� ������

        _TreeSpriteScale ("Tree Sprite Scale", Float) = 1.0 // ������� ������� ��������
        _Highlight ("HL_target", Color) = (0,0,0,1)
        _TintColor ("Tint Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        CGPROGRAM
        #pragma target 3.0 // ���������, ��� ������ ������� ��������� Shader Model 3.0
        #pragma surface surf Lambert vertex:vert  // ���������, ��� ���������� Surface Shader � Lambert ���������� � �������� ��������
        #include "Tessellation.cginc"
        sampler2D _ProvinceMap, _TerrainMap, _BumpMap, _DispMap;
        sampler2D _RiverMaskMap, _RiverTexture;
        sampler2D _TreeMaskMap, _PineTreeSprite, _TreeSprite, _PalmTreeSprite;
        float4 _Highlight;
        float4 _TintColor;

        struct Input
        {
            float2 uv_ProvinceMap;
            float2 uv_RiverMap;
            float2 uv_TreeMaskMap;
            float2 uv_TreeSprite;
        };

        float _Extrude;
        float _TreeSpriteScale;

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            v.vertex.xyz += v.normal * _Extrude * tex2Dlod(_DispMap, float4(v.texcoord.xy, 0.0, 0.0)).r;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // �������� ���� �� ����� ���������
            fixed4 col = tex2D(_ProvinceMap, IN.uv_ProvinceMap);
            // �������� ������� ��� ����������� ������
            fixed4 c1 = tex2D(_ProvinceMap, IN.uv_ProvinceMap + float2(0.0005, 0));
            fixed4 c2 = tex2D(_ProvinceMap, IN.uv_ProvinceMap - float2(0.0005, 0));
            fixed4 c3 = tex2D(_ProvinceMap, IN.uv_ProvinceMap + float2(0, 0.0005));
            fixed4 c4 = tex2D(_ProvinceMap, IN.uv_ProvinceMap - float2(0, 0.0005));
            fixed4 blackColor = fixed4(0, 0, 0, 1); // ������ ���� ��� ������
            float threshold = 0.01; // ����� ��� ��������� ������
            bool isBorder = any(abs(c1 - col) > threshold) || any(abs(c2 - col) > threshold) || any(abs(c3 - col) > threshold) || any(abs(c4 - col) > threshold);
            bool isTarget = col.rgb == _Highlight.rgb;
            
            // ���� ���������
            fixed4 terrainColor = tex2D(_TerrainMap, IN.uv_ProvinceMap);

            fixed3 mainMap = terrainColor.rgb;

            // ������ � ���������
            fixed3 treeMask = tex2D(_TreeMaskMap, IN.uv_ProvinceMap).rgb;
            fixed greenMaskValue = treeMask.g;
            fixed blueMaskValue = treeMask.b;
            fixed redMaskValue = treeMask.r;

            // ������������ UV ���������� ��� ������� � ��������� frac ��� ����������
            float2 scaledUV = IN.uv_TreeSprite * _TreeSpriteScale;
            float2 tiledUV = frac(scaledUV);

            // �������� �������� ��� ���� ����� ��������
            fixed4 pineTreeSprite = tex2D(_PineTreeSprite, tiledUV);
            fixed4 treeSprite = tex2D(_TreeSprite, tiledUV);
            fixed4 palmTreeSprite = tex2D(_PalmTreeSprite, tiledUV);

            // ������������� �������� �� ������ �������� �����
            fixed4 blendedSprite = 
                lerp(
                    lerp(palmTreeSprite, treeSprite, blueMaskValue),
                    pineTreeSprite, 
                    greenMaskValue);

            // ��������� ������ �������� ������ ���������
            fixed maskMaxValue = max(max(greenMaskValue, blueMaskValue), redMaskValue);

            // ��������� ���� ��� ���
            fixed3 finalColor = mainMap;

            // �����-�������� ��������
            finalColor = lerp(finalColor, blendedSprite.rgb, blendedSprite.a * maskMaxValue);

            // ������ � ������
            fixed3 riverMask = tex2D(_RiverMaskMap, IN.uv_ProvinceMap).rgb;
            fixed3 riverTexture = tex2D(_RiverTexture, IN.uv_RiverMap).rgb * riverMask.b;

            // ��������� ��� ������ ��������� � ��������
            finalColor = lerp(finalColor, riverTexture, riverMask.b);

            // ��������� ������ ������ ���������� �����
            if (isBorder)
            {
                finalColor = blackColor.rgb; // ���� �������, ������������� ������ ����
            }
            if (isTarget)
            {
                finalColor *= _TintColor; 
            }

            o.Albedo = finalColor;

            // ��������� ������� �� ���������� �����
            o.Normal = tex2D(_BumpMap, IN.uv_ProvinceMap);
        }
        ENDCG
    }
    FallBack "Diffuse" // �������� ������, ���� ������� �� ��������������
 
}
