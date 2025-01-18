using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Configs/Map Config")]
    public class MapConfig : ScriptableObject
    {
        [field: SerializeField] public Texture2D ProvinceTexture { get; private set; }
        [field: SerializeField] public float ChunkSize { get; private set; }
    }
}