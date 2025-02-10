using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Services.Storage;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderRenderer : MonoBehaviour
    {
        [SerializeField] private Texture2D _provinceTexture;
        [SerializeField] private MeshFilter _mesh;
        [SerializeField] private LineRenderer _lineRendererPrefab;
        [SerializeField] private Transform _lineContainer;
        
        [SerializeField] public Color32 _debugProvince;
        [SerializeField] public Color32 _oldDebugProvince;
        [SerializeField, Range(0, 3)] private int _debugMode = 0;
        [SerializeField, Range(0, 5)] private int _debugStep = 0;
        [SerializeField] bool _showDebug = false;
        [SerializeField] bool _showExecutionInfo = false;

        public List<IBorderCreationStep> _steps = new(10);
        public MapBorderData _data;
        
        public event Action OnInitialized;

        private IStorageService _storage = new BinaryStorageService();

        private async void Start()
        {
            _data = new(_provinceTexture, _mesh);
            
            
            if (_storage.HasFileByKey("BorderPoints"))
            {
                _steps = new()
                {
                    new LoadPointsStep(_data, _showExecutionInfo),
                    new DrawStep(_data, _lineRendererPrefab, _lineContainer, _showExecutionInfo),
                };
            }
            else
            {
                _steps = new()
                {
                    new BorderPixelsCollectionStep(_data, _showExecutionInfo),
                    new PointsCreationStep(_data, _showExecutionInfo),
                    new SortPointsStep(_data, _showExecutionInfo),
                    new FilterStraightPointsStep(_data, _showExecutionInfo),
                    new SavePointsStep(_data, _showExecutionInfo),
                    new DrawStep(_data, _lineRendererPrefab, _lineContainer, _showExecutionInfo),
                };
            }
                
            foreach(var step in _steps)
            {
                await step.Execute();
            }

            _steps = null;
            _data = null;
            
            OnInitialized?.Invoke();
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) return;
            if(_steps.Count == 0) return;
            if(_debugStep > _steps.Count - 1)return;
            if(_showDebug == false) return;
            
            var step = _steps[_debugStep];
            step?.DrawGizmos(_debugProvince, _oldDebugProvince, _debugMode);
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {           
                var mousePos = Input.mousePosition;
                var ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    var p = hitInfo.textureCoord;
                    int x = (int)Mathf.Floor(p.x * _data.TextureWidth);
                    int y = (int)Mathf.Floor(p.y * _data.TextureHeight);

                    _oldDebugProvince = _debugProvince;
                    _debugProvince = _provinceTexture.GetPixel(x, y);
                }  
            }
 
        }

        [ContextMenu("TST")]
        private void TST()
        { 
            
            
        }
    }
}