using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Configs;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.MapRenderer
{
    public class ChunkSystem
    {
        private BorderRenderCamera _borderRenderCamera;
        private MapConfig _config;
        private Transform _chunkContainer;
        private List<List<MapChunk>> _chunks;
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
            //BakeChunksBorders();
        }

        private void CreateChunks()
        {
            var startOffset = _config.MapStartPoint + new Vector3(_chunkSize / 2f, _chunkSize / 2f);
            
            _chunks = new List<List<MapChunk>>(_chunkCount.y);

            for (int y = 0; y < _chunkCount.y; y++)
            {
                _chunks.Add(new List<MapChunk>(_chunkCount.x));
                
                for (int x = 0; x < _chunkCount.x; x++)
                {
                    var chunk = Object.Instantiate(_config.ChunkPrefab, _chunkContainer);
                    chunk.name = $"Chunk {x}, {y}";
                    chunk.transform.position = startOffset + new Vector3(x * _chunkSize, y * _chunkSize);
                    
                    Vector4 tilingOffset = new Vector4(_chunkTilling.x, _chunkTilling.y, _chunkTilling.x * x, _chunkTilling.y * y); 
                    chunk.SetTilingAndOffset(tilingOffset);

                    chunk.IsVisible.Subscribe(_ => BakeChunk(chunk)).AddTo(chunk);
                    
                    _chunks[y].Add(chunk);
                }
            }
        }
        
        private async void BakeChunksBorders()
        {
            foreach (var chunkLine in _chunks)
            {
                foreach (var chunk in chunkLine)
                {
                    _borderRenderCamera.RenderAsync(chunk.transform.position, chunk.SetBorders, _chunkSize * 0.5f);
                    await Task.Yield();
                }
            }
        }

        private void BakeChunk(MapChunk chunk)
        {
            if (chunk.IsVisible.CurrentValue)
            {
                _borderRenderCamera.RenderAsync(chunk.transform.position, chunk.SetBorders, _chunkSize * 0.5f); 
            }
        }
    }
}