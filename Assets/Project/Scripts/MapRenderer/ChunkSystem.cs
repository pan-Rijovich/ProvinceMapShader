using System;
using System.Collections.Generic;
using Project.Scripts.Configs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.MapRenderer
{
    public class ChunkSystem
    {
        private BorderRenderCamera _borderRenderCamera;
        private MapConfig _config;
        private Transform _chunkContainer;
        private List<List<MeshRenderer>> _chunks;
        private Vector2 _chunkTilling;
        private Vector2Int _chunkCount;
        private int _chunkSize = 128;

        public ChunkSystem(BorderRenderCamera borderRenderCamera, MapConfig config)
        {
            _borderRenderCamera = borderRenderCamera;
            _config = config;

            _chunkContainer = new GameObject("Chunks").transform;
            _chunkCount.x = (int)MathF.Ceiling((float)_config.MapSizeInWorld.x / _chunkSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_config.MapSizeInWorld.y / _chunkSize);
            _chunkTilling.x = 1f / _chunkCount.x;
            _chunkTilling.y = 1f / _chunkCount.y;

            CreateChunks();
            BakeChunkBorders();
        }

        private void CreateChunks()
        {
            var startOffset = _config.MapStartPoint + new Vector3(_chunkSize / 2f, _chunkSize / 2f);
            
            _chunks = new List<List<MeshRenderer>>(_chunkCount.y);

            for (int y = 0; y < _chunkCount.y; y++)
            {
                _chunks.Add(new List<MeshRenderer>(_chunkCount.x));
                
                for (int x = 0; x < _chunkCount.x; x++)
                {
                    var chunk = Object.Instantiate(_config.ChunkPrefab, _chunkContainer);
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