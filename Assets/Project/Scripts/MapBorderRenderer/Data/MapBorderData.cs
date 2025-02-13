﻿using System.Collections.Generic;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderData
    {
        public Dictionary<int, BorderPixelsCollection> BorderPixels { get; set; } = new(5000);
        public Dictionary<long, BorderCreationData> BordersCreationData { get; set; } = new();
        public BorderSaveData[] BordersSaveData;

        public Color32[] TextureArr { get; set; }
        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }
        public Vector3 MeshSize { get; private set; }
        public Bounds MeshBounds { get; private set; }

        public MapBorderData(Texture2D provinceTexture, MeshFilter mesh)
        {
            TextureArr = provinceTexture.GetPixels32();
            TextureWidth = provinceTexture.width;
            TextureHeight = provinceTexture.height;

            if (mesh.sharedMesh != null)
            {
                var transform = mesh.transform;
                MeshBounds = mesh.sharedMesh.bounds;
                MeshSize += mesh.sharedMesh.bounds.size;
                MeshSize = new Vector3(MeshSize.x * transform.localScale.x, 
                    MeshSize.y * transform.localScale.y, 
                    MeshSize.z * transform.localScale.z);
            }
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
            if(leftIndex % TextureWidth == TextureWidth - 1) leftIndex += TextureWidth;
            if(currentIndex == 0) leftIndex = TextureWidth - 1;
            return leftIndex;
        }
                
        public int GetRightIndex(int currentIndex)
        {
            var rightIndex = currentIndex + 1;
            if(rightIndex % TextureWidth == 0) rightIndex -= TextureWidth;
                    
            return rightIndex;
        }
                
        public int GetUpIndex(int currentIndex)
        {                                
            if (currentIndex + TextureWidth < TextureArr.Length)
            {
                return currentIndex + TextureWidth;
            }
            return currentIndex;
        }
                
        public int GetDownIndex(int currentIndex)
        {                             
            if (currentIndex - TextureWidth >= 0)
            {
                return currentIndex - TextureWidth;
            }
            return currentIndex;
        }
                
        public Vector2 ConvertIndexToFloatPixelCoordinated(int index)
        {          
            var result = Vector2.zero;
            result.x = index % TextureWidth;
            result.y = index / TextureWidth;
                    
            return result;
        }
               
        public Vector2Int ConvertIndexToIntPixelCoordinated(int index)
        {          
            var result = Vector2Int.zero;
            result.x = index % TextureWidth;
            result.y = index / TextureWidth;
                    
            return result;
        }
#endregion
    }
}