using System.Collections.Generic;
using System.Diagnostics;
using Project.Scripts.Configs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapRenderer
{
    public class RemapBaker
    {
        private MapConfig _config;
        private Texture2D _remap;
        private Texture2D _province;
        private Color32[] remapArr;
        private int width;
        private int height;
        private bool _showLogs;

        public RemapBaker(MapConfig config)
        {
            _config = config;
            _province = _config.ProvinceTexture;
            
            width = _province.width;
            height = _province.height;
            
            _remap = new Texture2D(width, height, TextureFormat.RG16, false);
            _remap.filterMode = FilterMode.Point;
            _config.RemapTexture = _remap;

            CalcRemap();
        }


        private void CalcRemap()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            byte[] provinceByteArr = _province.GetRawTextureData();
            byte[] remapByteArr = new byte[(provinceByteArr.Length / 3) * 2];

            int remapIdx = 0;
            int uniqueProvinceIdx = 0;
            Dictionary<int, short> main2remap = new(4000);

            for (int i = 0; i < provinceByteArr.Length; i += 3)
            {
                int provinceColor24 = provinceByteArr[i + 2] << 16 | provinceByteArr[i + 1] << 8 | provinceByteArr[i];
                short remapColorRG16;
                if (!main2remap.ContainsKey(provinceColor24))
                {
                    byte low = (byte)(uniqueProvinceIdx & 0xff); // mod 256
                    byte high = (byte)(uniqueProvinceIdx >> 8); // div 256
                    remapColorRG16 = (short)(low | high << 8);
                    main2remap[provinceColor24] = remapColorRG16;
                    uniqueProvinceIdx++;
                }
                else
                {
                    remapColorRG16 = main2remap[provinceColor24];
                }
                remapByteArr[remapIdx++] = (byte)remapColorRG16;
                remapByteArr[remapIdx++] = (byte)(remapColorRG16 >> 8);
            }

            var calcTime = stopwatch.ElapsedMilliseconds;
            if(_showLogs) Debug.Log($"remap calc time:{calcTime}\n");

            _remap.LoadRawTextureData(remapByteArr);
            _remap.Apply(false);

            var fillTime = stopwatch.ElapsedMilliseconds - calcTime;
            if(_showLogs) Debug.Log($"fill remapTex time:{fillTime}\n");

            remapArr = new Color32[remapByteArr.Length / 2];
            for (int i = 0; i < remapByteArr.Length / 2; i++)
            {
                remapArr[i] = new Color32(remapByteArr[i << 1], remapByteArr[(i << 1) + 1], 0, 255);
            }

            if(_showLogs) Debug.Log($"fill remapArr time:{stopwatch.ElapsedMilliseconds - fillTime - calcTime}\n");

            stopwatch.Stop();
            if(_showLogs) Debug.Log($"total time:{stopwatch.ElapsedMilliseconds}\n");
        }
    }
}