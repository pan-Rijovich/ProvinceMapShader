using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts.MapRenderer
{
    public class MapRenderer : MonoBehaviour
    {
        [SerializeField] private Vector2Int _mapSize = new Vector2Int(5632, 2048);
        [SerializeField] private int _chunckSize = 128;
        [SerializeField] private MeshRenderer _quadPrefab;
        
        private List<List<MeshRenderer>> _chunks;
        
        private Vector2 _chunkTilling;
        private Vector2Int _chunkCount;

        private void Awake()
        {
            _chunkCount.x = (int)MathF.Ceiling((float)_mapSize.x / _chunckSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_mapSize.y / _chunckSize);
            _chunkTilling.x = 1f / _chunkCount.x;
            _chunkTilling.y = 1f / _chunkCount.y;
            
            var startOffset = new Vector3(-_mapSize.x / 2f, -_mapSize.y / 2f);
            
            _chunks = new List<List<MeshRenderer>>(_chunkCount.y);

            for (int y = 0; y < _chunkCount.y; y++)
            {
                _chunks.Add(new List<MeshRenderer>(_chunkCount.x));
                
                for (int x = 0; x < _chunkCount.x; x++)
                {
                    var chunk = Instantiate(_quadPrefab, transform);
                    chunk.name = $"Chunk {x}, {y}";
                    chunk.transform.position = startOffset + new Vector3(x * _chunckSize, y * _chunckSize);
                    
                    var block = new MaterialPropertyBlock();
                    Vector4 tilingOffset = new Vector4(_chunkTilling.x, _chunkTilling.y, _chunkTilling.x * x, _chunkTilling.y * y); 
                    //block.SetVector("_NormalMap_ST", tilingOffset);
                    //block.SetVector("_ProvinceTex_ST", tilingOffset);
                    block.SetVector("_Tilling", tilingOffset);
                    chunk.SetPropertyBlock(block);
                    
                    _chunks[y].Add(chunk);
                }
            }
        }
    }
}