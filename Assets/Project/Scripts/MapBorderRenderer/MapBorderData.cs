using System.Collections.Generic;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderData
    {
        public Dictionary<long, Border> Borders { get; set; } = new();
        public Dictionary<uint, List<long>> ProvincesBorders { get; set; } = new();
        public List<List<BorderPoint>> DeletedLines { get; set; } = new();

        public uint _gizmosProvince { get; set; } = 0;
        public List<LineRenderer> _lineRenderer { get; set; } = new();

        public Texture2D Texture { get; private set; }
        public Color32[] TextureArr { get; private set; }
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
    }
}