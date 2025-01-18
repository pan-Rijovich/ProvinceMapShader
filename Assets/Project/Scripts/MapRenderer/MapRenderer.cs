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
        
        private Vector2 _chunkTillingOffset;
        private Vector2Int _chunkCount;

        private void Awake()
        {
            _chunkTillingOffset.x = (float)_mapSize.x / _chunckSize;
            _chunkTillingOffset.y = (float)_mapSize.y / _chunckSize;
            _chunkCount.x = (int)MathF.Ceiling((float)_mapSize.x / _chunckSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_mapSize.y / _chunckSize);
            
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
                    
                    _chunks[y].Add(chunk);
                }
            }
        }
    }
}