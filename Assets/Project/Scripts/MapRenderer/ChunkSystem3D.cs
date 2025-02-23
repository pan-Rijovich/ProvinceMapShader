using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Configs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.MapRenderer
{
    public class ChunkSystem3D
    {
        private BorderBaker _borderBaker;
        private MapConfig _config;
        private Transform _chunkContainer;
        private List<List<MeshRenderer>> _chunks;
        private Vector2 _chunkTilling;
        private Vector2Int _chunkCount;
        private int _chunkSize = 128;
        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;
        private Color[] _colors;

        public ChunkSystem3D(BorderBaker borderBaker, MapConfig config)
        {
            _borderBaker = borderBaker;
            _config = config;

            _chunkContainer = new GameObject("Chunks3D").transform;
            _chunkCount.x = (int)MathF.Ceiling((float)_config.MapSizeInWorld.x / _chunkSize);
            _chunkCount.y = (int)MathF.Ceiling((float)_config.MapSizeInWorld.y / _chunkSize);
            _chunkTilling.x = 1f / _chunkCount.x;
            _chunkTilling.y = 1f / _chunkCount.y;

            CreateChunks();
            BakeChunkBorders();
        }
        
        private async void CreateChunks()
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
                    chunk.transform.position = new Vector3(x * _chunkSize, y * _chunkSize);
                    
                    var block = new MaterialPropertyBlock();
                    Vector4 tilingOffset = new Vector4(_chunkTilling.x, _chunkTilling.y, _chunkTilling.x * x, _chunkTilling.y * y); 
                    block.SetVector("_Tilling", tilingOffset);
                    //chunk.SetPropertyBlock(block);
                    
                    //_chunks[y].Add(chunk);
                    
                    GenerateMapMesh(chunk.GetComponent<MeshFilter>(), tilingOffset);

                    await Task.Yield();
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

                    //var texture = _borderRenderCamera.Render(chunk.transform.position, _chunkSize * 0.5f);
                    //block.SetTexture("_BorderTexture", texture);
                    
                    chunk.SetPropertyBlock(block);
                }
            }
        }
        
        private void GenerateMapMesh(MeshFilter filter, Vector4 tilingOffset)
        {
            var mesh = new Mesh();
            filter.mesh = mesh;
            Vector2 tilling = new Vector2(tilingOffset.x, tilingOffset.y);
            Vector2 offset = new Vector2(tilingOffset.z, tilingOffset.w);
            

            GenerateVertices(tilingOffset);
            GenerateFaces();
            GenerateUVs(tilingOffset);
            GenerateColors(tilingOffset);
            
            UpdateMesh(mesh);
        }

        private void GenerateVertices(Vector4 tilingOffset)
        {
            _vertices = new Vector3[(_chunkSize + 1) * (_chunkSize + 1)];
            var quadSize = 1f;

            int i = 0;
            for (var y = 0; y <= _chunkSize; y++)
            {
                for (var x = 0; x <= _chunkSize; x++)
                {
                    var uv = GetUV(x, y, tilingOffset);
                    var height = GetRandomVertexHeight(uv);
                    
                    _vertices[i] = new Vector3(x * quadSize, y * quadSize, height);
                    i++;
                }
            }
        }

        private void GenerateFaces()
        {
            _triangles = new int[_chunkSize * _chunkSize * 6];

            int vert = 0;
            int tris = 0;
            for (var y = 0; y < _chunkSize; y++)
            {
                for (var x = 0; x < _chunkSize; x++) {
                    
                    _triangles[tris + 0] = vert + 0;
                    _triangles[tris + 1] = vert + _chunkSize + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + _chunkSize + 1;
                    _triangles[tris + 5] = vert + _chunkSize + 2;
                    
                    vert++;
                    tris += 6;
                }
                vert++;
            }
        }

        private void GenerateUVs(Vector4 tilingOffset)
        {
            _uvs = new Vector2[_vertices.Length];

            int i = 0;
            for (var y = 0; y <= _chunkSize; y++)
            {
                for (var x = 0; x <= _chunkSize; x++)
                {
                    _uvs[i] = GetUV(x, y, tilingOffset);
                    i++;
                }
            }
        }
        
        private void GenerateColors(Vector4 tilingOffset)
        {
            _colors = new Color[_vertices.Length];

            int i = 0;
            for (var y = 0; y <= _chunkSize; y++)
            {
                for (var x = 0; x <= _chunkSize; x++)
                {
                    var uv = GetUV(x, y, tilingOffset);
                    var color = GetPixelColor(_config.TerrainTexture, uv);
                    _colors[i] = color;


                    //var t = (_vertices[i].z + 25f) / 50f;
                    //_colors[i] = Color.Lerp(Color.white, Color.black, t);
                    i++;
                }
            }
        }
        
        private void UpdateMesh(Mesh mesh)
        {
            if(mesh == null) return;
            mesh.Clear();
            mesh.vertices = _vertices;
            mesh.triangles = _triangles;
            //mesh.uv = _uvs;
            mesh.colors = _colors;
            mesh.RecalculateNormals();
        }

        private float GetRandomVertexHeight(Vector2 uv)
        {
            return Mathf.Lerp(10f, -10f, GetPixelColor(_config.HeightTexture, uv).r);
        }
        
        public Vector2 GetUV(int x, int y, Vector4 tilingOffset)
        {
            float u = (x / (float)_chunkSize) * tilingOffset.x + tilingOffset.z;
            float v = (y / (float)_chunkSize) * tilingOffset.y + tilingOffset.w;
    
            return new Vector2(u, v);
        }
        
        public Color GetPixelColor(Texture2D texture, Vector2 uv)
        {
            if (texture == null) return Color.magenta;

            // Конвертуємо UV у координати пікселя текстури
            int x = Mathf.Clamp(Mathf.FloorToInt(uv.x * texture.width), 0, texture.width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(uv.y * texture.height), 0, texture.height - 1);

            return texture.GetPixel(x, y);
        }
    }
}