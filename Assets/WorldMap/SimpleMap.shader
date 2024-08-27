Shader "Custom/SimpleMap"
{
	Properties
    {
        _ProvinceMap ("Province Map", 2D) = "white" {}
        _TerrainMap ("Terrain Map", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}

        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderSize ("Border Size", Range(0.0001, 0.01)) = 0.001

        _RegionColors("Region Colors", 2D) = "white" {}
        _int("int", Range(0,256)) = 0
    }
    SubShader
    {
        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert
        sampler2D _ProvinceMap, _TerrainMap, _BumpMap, _RegionColors;
        fixed4 _BorderColor;
        float _BorderSize;
        int _int;

        struct Input
        {
            float2 uv_ProvinceMap;
            float2 uv_RegionColors;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Основной цвет из карты провинций
            fixed4 col = tex2D(_ProvinceMap, IN.uv_ProvinceMap);
            // Соседние пиксели для определения границ
            fixed4 c1 = tex2D(_ProvinceMap, IN.uv_ProvinceMap + float2(_BorderSize, 0));
            fixed4 c2 = tex2D(_ProvinceMap, IN.uv_ProvinceMap - float2(_BorderSize, 0));
            fixed4 c3 = tex2D(_ProvinceMap, IN.uv_ProvinceMap + float2(0, _BorderSize));
            fixed4 c4 = tex2D(_ProvinceMap, IN.uv_ProvinceMap - float2(0, _BorderSize));
            fixed4 blackColor = fixed4(0, 0, 0, 1); // Черный цвет для границ
            float threshold = 0.01; // Порог для сравнения цветов
            bool isBorder = any(abs(c1 - col) > threshold) || any(abs(c2 - col) > threshold) || any(abs(c3 - col) > threshold) || any(abs(c4 - col) > threshold);
            
            // Цвет ландшафта
            fixed4 terrainColor = tex2D(_TerrainMap, IN.uv_ProvinceMap);

            fixed3 mainMap = terrainColor.rgb;

            // Финальный цвет без рек
            fixed3 finalColor = mainMap;

            // Установка границ поверх финального цвета
            if (isBorder)
            {
               finalColor = fixed4(0,0,0,1);
            }
            else
            {
               o.Normal = tex2D(_BumpMap, IN.uv_ProvinceMap);
            }
            o.Albedo = finalColor;

        }

        ENDCG
    }
    FallBack "Diffuse" // Запасной шейдер, если текущий не поддерживается
 
}
