using Project.Scripts.MapRenderer;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "MapTextureConfig", menuName = "Configs/MapTextureConfig")]
    public class MapConfig : ScriptableObject
    {
        public Texture2D TerrainTexture;
        public Texture2D ProvinceTexture;
        public Texture2D NormalTexture;
        public Texture2D HeightTexture;
        public Texture2D RemapTexture;
        public Texture2D ProvincePalette;
        public Texture2D BorderPalette;
        
        public Vector3 MapSizeInWorld;
        public Vector3 MapStartPoint;

        public LineRenderer BorderPrefab;
        public MapChunk ChunkPrefab;

    }
}