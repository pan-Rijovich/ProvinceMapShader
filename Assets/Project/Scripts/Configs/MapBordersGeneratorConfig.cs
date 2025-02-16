using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "MapBordersGeneratorConfig", menuName = "Configs/MapBordersGeneratorConfig")]
    public class MapBordersGeneratorConfig : ScriptableObject
    {
        public Color32 DebugProvince;
        public Color32 OldDebugProvince;
        [Range(0, 3)] public int DebugMode = 0;
        [Range(0, 5)] public int DebugStep = 0;
        public bool ShowDebug = false;
        public bool ShowExecutionInfo = false;
    }
}