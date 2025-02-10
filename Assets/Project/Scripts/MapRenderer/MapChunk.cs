using System;
using UnityEngine;

namespace Project.Scripts.MapRenderer
{
    public class MapChunk : MonoBehaviour
    {
        [SerializeField] private LODGroup _lodGroup;

        private void Awake()
        {
            var lod = new LOD();
            
            _lodGroup.SetLODs(_lodGroup.GetLODs());
        }
    }
}