using System.Collections.Generic;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderData
    {
        public Dictionary<int, BorderPixelsCollection> BorderPixels { get; set; } = new(5000);
        public Dictionary<long, Border> Borders { get; set; } = new();
        public Dictionary<uint, List<long>> ProvincesBorders { get; set; } = new();
        public List<List<BorderPoint>> DeletedLines { get; set; } = new();

        public List<LineRenderer> _lineRenderer { get; set; } = new();

        public Texture2D Texture { get; private set; }
        public Color32[] TextureArr { get; set; }
        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }
        public Vector3 MeshSize { get; private set; }
        public Material Material { get; private set; }
        public Mesh Mesh { get; private set; }
        public Transform Transform { get; private set; }

        public MapBorderData(Material material, Mesh mesh, Transform transform)
        {
            Material = material;
            Mesh = mesh;
            Transform = transform;

            Texture = Material.GetTexture("_ProvinceTex") as Texture2D;
            TextureArr = Texture.GetPixels32();

            TextureWidth = Texture.width;
            TextureHeight = Texture.height;

            CalcMeshSize();
        }
        
        
        public long GenerateBorderID(Color32 color1, Color32 color2)
        {
            long id;
            if (color1.LessThanColor(color2))
            {
                id = ((long)color2.Color32ToUInt() << 32) + color1.Color32ToUInt();
            }
            else
            {
                id = ((long)color1.Color32ToUInt() << 32) + color2.Color32ToUInt();
            }
            return id;
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
        
        public int Color32ToInt(Color32 color)
        {
            return (color.r << 24) | (color.g << 16) | (color.b << 8) | color.a;
        }
        
        public Color32 IntToColor32(int value)
        {
            byte r = (byte)((value >> 24) & 0xFF);
            byte g = (byte)((value >> 16) & 0xFF);
            byte b = (byte)((value >> 8) & 0xFF);
            byte a = (byte)(value & 0xFF);

            return new Color32(r, g, b, a);
        }

        #region LEGACY
        public void AddToListInDictionary<T, U>(T key, U value, Dictionary<T, List<U>> dictionary)
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                list = new List<U>();
                dictionary[key] = list;
            }

            list.Add(value);
        }


        private void CalcMeshSize()
        {
            if (Mesh != null)
            {
                Vector3 vector3 = Vector3.zero;
                vector3 += Mesh.bounds.size;
                vector3 = new Vector3(vector3.x * Transform.localScale.x, vector3.y * Transform.localScale.y, vector3.z * Transform.localScale.z);
                MeshSize = vector3;
            }
        }
        #endregion

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