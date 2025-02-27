using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.MapBorderRenderer;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.MapRenderer
{
    public class ChunkSystem
    {
        private ChunkRenderSettingsProvider _settingsProvider;
        private BorderBaker _borderBaker;
        private MapConfig _config;
        private Transform _chunkContainer;
        private List<List<MapChunk>> _chunks;
        private Vector2 _chunkTilling;
        private Vector2Int _chunkCount;
        private int _chunkSize = 128;

        public ChunkSystem(BorderBaker borderBaker, MapConfig config, ChunkRenderSettingsProvider settingsProvider)
        {
            _borderBaker = borderBaker;
            _config = config;
            _settingsProvider = settingsProvider;

            _chunkContainer = new GameObject("Chunks").transform;
            _chunkCount.x = (int)MathF.Ceiling((float)_config.MapSizeInWorld.x / _chunkSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_config.MapSizeInWorld.y / _chunkSize);
            _chunkTilling.x = 1f / _chunkCount.x;
            _chunkTilling.y = 1f / _chunkCount.y;

            CreateChunks();
            //BakeChunksBorders();

            _settingsProvider.BorderTextureResolution.Subscribe(_ => ReBakeActiveChunks());
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
                    _borderBaker.RenderAsync(chunk.transform.position, chunk.SetBorders, _chunkSize * 0.5f);
                    await Task.Yield();
                }
            }
        }

        private void BakeChunk(MapChunk chunk)
        {
            if (chunk.IsVisible.CurrentValue)
            {
                _borderBaker.RenderAsync(chunk.transform.position, chunk.SetBorders, _chunkSize * 0.5f); 
            }
        }

        private void ReBakeActiveChunks()
        {
            foreach (var chunkLine in _chunks)
            {
                foreach (var chunk in chunkLine)
                {
                    if(chunk.IsVisible.CurrentValue) _borderBaker.RenderAsync(chunk.transform.position, chunk.SetBorders, _chunkSize * 0.5f);
                    
                }
            }
        }

        private bool TryGetChunk(int x, int y, out MapChunk chunk)
        {
            chunk = null;
            if(_chunks == null) return false;
            if(_chunks.Count < y + 1) return false;
            if(_chunks[y].Count < x + 1) return false;
            
            chunk = _chunks[y][x];
            return true;
        }
        
        private bool TryGetChunk(Vector2Int coordinates, out MapChunk chunk)
        {
            return TryGetChunk(coordinates.x, coordinates.y, out chunk);
        }
    }
}