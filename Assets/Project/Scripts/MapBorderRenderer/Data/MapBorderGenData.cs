using System.Collections.Generic;
using Project.Scripts.Configs;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderGenData
    {
        public Dictionary<int, BorderPixelsCollection> BorderPixels { get; set; } = new(5000);
        public Dictionary<long, BorderCreationData> BordersCreationData { get; set; } = new();
        public BorderSaveData[] BordersSaveData;

        public Color32[] TexPixels { get; set; }
        public int TexWidth { get; private set; }
        public int TexHeight { get; private set; }
        public Vector3 MapSize { get; private set; }
        public Vector3 MapStartPoint { get; private set; }

        public MapBorderGenData(MapConfig mapConfig)
        {
            TexPixels = mapConfig.ProvinceTexture.GetPixels32();
            TexWidth = mapConfig.ProvinceTexture.width;
            TexHeight = mapConfig.ProvinceTexture.height;
            MapSize = mapConfig.MapSizeInWorld;
            MapStartPoint = mapConfig.MapStartPoint;
        }
        
        public long GenerateBorderID(int color1, int color2)
        {
            long id;
            if (color1 < color2)
            {
                id = ((long)color2 << 32) + color1;
            }
            else
            {
                id = ((long)color1 << 32) + color2;
            }
            return id;
        }

        #region INDEX_CALCULATIONS
        public int GetLeftIndex(int currentIndex)
        {
            var leftIndex = currentIndex - 1;
            if(leftIndex % TexWidth == TexWidth - 1) leftIndex += TexWidth;
            if(currentIndex == 0) leftIndex = TexWidth - 1;
            return leftIndex;
        }
                
        public int GetRightIndex(int currentIndex)
        {
            var rightIndex = currentIndex + 1;
            if(rightIndex % TexWidth == 0) rightIndex -= TexWidth;
                    
            return rightIndex;
        }
                
        public int GetUpIndex(int currentIndex)
        {                                
            if (currentIndex + TexWidth < TexPixels.Length)
            {
                return currentIndex + TexWidth;
            }
            return currentIndex;
        }
                
        public int GetDownIndex(int currentIndex)
        {                             
            if (currentIndex - TexWidth >= 0)
            {
                return currentIndex - TexWidth;
            }
            return currentIndex;
        }
                
        public Vector2 ConvertIndexToFloatPixelCoordinated(int index)
        {          
            var result = Vector2.zero;
            result.x = index % TexWidth;
            result.y = index / TexWidth;
                    
            return result;
        }
               
        public Vector2Int ConvertIndexToIntPixelCoordinated(int index)
        {          
            var result = Vector2Int.zero;
            result.x = index % TexWidth;
            result.y = index / TexWidth;
                    
            return result;
        }
#endregion
    }
}