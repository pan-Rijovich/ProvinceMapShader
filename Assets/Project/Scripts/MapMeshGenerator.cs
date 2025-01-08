using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapMeshGenerator : MonoBehaviour
    {
        [SerializeField] Vector2Int _mapSize;
        [SerializeField] Vector2 _minMaxHeight;
        [SerializeField] float _quadSize;
        [SerializeField] Gradient _painting;
        
        private MeshFilter _meshFilter;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;
        private Color[] _colors;
        
        [Button]
        private void Clear()
        {
            GetComponent<MeshFilter>().sharedMesh.Clear();
        }
        
        [Button]
        private void GenerateMapMesh()
        {
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;

            GenerateVertices();
            GenerateFaces();
            GenerateUVs();
            GenerateColors();
            
            UpdateMesh();
        }

        private void GenerateVertices()
        {
            _vertices = new Vector3[(_mapSize.x + 1) * (_mapSize.y + 1)];

            int i = 0;
            for (var y = 0; y <= _mapSize.y; y++)
            {
                for (var x = 0; x <= _mapSize.x; x++)
                {
                    //var height = 0f;
                    var height = GetRandomVertexHeight(x, y);
                    
                    _vertices[i] = new Vector3(x * _quadSize, y * _quadSize, height);
                    i++;
                }
            }
        }

        private void GenerateFaces()
        {
            _triangles = new int[_mapSize.x * _mapSize.y * 6];

            int vert = 0;
            int tris = 0;
            for (var y = 0; y < _mapSize.y; y++)
            {
                for (var x = 0; x < _mapSize.x; x++) {
                    
                    _triangles[tris + 0] = vert + 0;
                    _triangles[tris + 1] = vert + _mapSize.x + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + _mapSize.x + 1;
                    _triangles[tris + 5] = vert + _mapSize.x + 2;
                    
                    vert++;
                    tris += 6;
                }
                vert++;
            }
        }

        private void GenerateUVs()
        {
            _uvs = new Vector2[_vertices.Length];

            int i = 0;
            for (var y = 0; y <= _mapSize.y; y++)
            {
                for (var x = 0; x <= _mapSize.x; x++)
                {
                    _uvs[i] = new Vector2((float)x / _mapSize.x, (float)y / _mapSize.y);
                    i++;
                }
            }
        }
        
        private void GenerateColors()
        {
            _colors = new Color[_vertices.Length];

            int i = 0;
            for (var y = 0; y <= _mapSize.y; y++)
            {
                for (var x = 0; x <= _mapSize.x; x++)
                {
                    var value = Mathf.InverseLerp(_minMaxHeight.x, _minMaxHeight.y, _vertices[i].z);
                    _colors[i] = _painting.Evaluate(value);
                    i++;
                }
            }
        }
        
        private void UpdateMesh()
        {
            if(_mesh == null) return;
            _mesh.Clear();
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            //_mesh.uv = _uvs;
            _mesh.colors = _colors;
            _mesh.RecalculateNormals();
        }

        private float GetRandomVertexHeight(int x, int y)
        {
            return Mathf.Lerp(_minMaxHeight.x, _minMaxHeight.y, Mathf.PerlinNoise(x * 0.3f, y * 0.3f));
        }

        /*private void OnDrawGizmos()
        {
            if(_vertices == null) return;
            foreach (var v in _vertices)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }*/
        
        

        /*private void Update()
        {
            UpdateMesh();
        }*/
    }
}