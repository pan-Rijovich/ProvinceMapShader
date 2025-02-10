using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.MapRenderer
{
    public class ChunkSystem : MonoBehaviour
    {
        [SerializeField] private Vector2Int _mapSize = new Vector2Int(5632, 2048);
        [SerializeField] private int _chunkSize = 128;
        [SerializeField] private MeshRenderer _quadPrefab;
        [SerializeField] private BorderRenderCamera _borderRenderCamera;
        
        private List<List<MeshRenderer>> _chunks;
        
        private Vector2 _chunkTilling;
        private Vector2Int _chunkCount;

        private void Awake()
        {
            _chunkCount.x = (int)MathF.Ceiling((float)_mapSize.x / _chunkSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_mapSize.y / _chunkSize);
            _chunkTilling.x = 1f / _chunkCount.x;
            _chunkTilling.y = 1f / _chunkCount.y;
            
            var startOffset = new Vector3(-_mapSize.x / 2f, -_mapSize.y / 2f) + new Vector3(_chunkSize / 2f, _chunkSize / 2f);
            
            _chunks = new List<List<MeshRenderer>>(_chunkCount.y);

            for (int y = 0; y < _chunkCount.y; y++)
            {
                _chunks.Add(new List<MeshRenderer>(_chunkCount.x));
                
                for (int x = 0; x < _chunkCount.x; x++)
                {
                    var chunk = Instantiate(_quadPrefab, transform);
                    chunk.name = $"Chunk {x}, {y}";
                    chunk.transform.position = startOffset + new Vector3(x * _chunkSize, y * _chunkSize);
                    
                    var block = new MaterialPropertyBlock();
                    Vector4 tilingOffset = new Vector4(_chunkTilling.x, _chunkTilling.y, _chunkTilling.x * x, _chunkTilling.y * y); 
                    block.SetVector("_Tilling", tilingOffset);
                    chunk.SetPropertyBlock(block);
                    
                    _chunks[y].Add(chunk);
                }
            }
        }

        [Button]
        private void BakeChunkBorders()
        {
            foreach (var chunkLine in _chunks)
            {
                foreach (var chunk in chunkLine)
                {
                    var block  = new MaterialPropertyBlock();
                    chunk.GetPropertyBlock(block);

                    var texture = _borderRenderCamera.Render(chunk.transform.position, _chunkSize * 0.5f);
                    block.SetTexture("_BorderTexture", texture);
                    
                    chunk.SetPropertyBlock(block);
                }
            }
        }
    }
}