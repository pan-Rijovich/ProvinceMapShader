using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapBorderRenderer
{
    public class MapBorderRenderer : MonoBehaviour
    {
        [SerializeField] bool _showExecutionInfo = false;
        [SerializeField] private LineRenderer _lineRendererPrefab;
        [SerializeField] private LineRenderer _lineRendererTST;
        [SerializeField] private Color32 _debugProvince;
        [SerializeField] private Color32 _oldDebugProvince;
        [SerializeField, Range(0, 3)] private int _debugMode = 0;
        [SerializeField, Range(0, 5)] private int _debugStep = 0;

        private List<IBorderCreationStep> _steps = new(10);
        private MapBorderData _data;
        

        private async void Start()
        {
            _data = new(GetComponent<Renderer>().material, GetComponent<MeshFilter>().mesh, transform);

            _steps = new()
            {
                new BorderPixelsCollectionStep(_data, _showExecutionInfo),
                new PointsCreationStep(_data, _showExecutionInfo),
                new SortPointsStep(_data, _showExecutionInfo),
                //new FilterStraightPointsStep(_data, _showExecutionInfo),
            };

            foreach(var step in _steps)
            {
                await step.Execute();
            }
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) return;
            if(_steps.Count == 0) return;
            if(_debugStep > _steps.Count - 1)return;
            
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
                    _debugProvince = _data.Texture.GetPixel(x, y);
                }  
            }
 
        }

        [ContextMenu("TST")]
        private void TST()
        { 
            Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2);
            var id = _data.GenerateBorderID(_debugProvince, _oldDebugProvince);
            if (_data.Borders.TryGetValue(id, out var border))
            {
                var subborder = border[0];
                Vector3[] array = new Vector3[subborder.SortedPoints.Count];
                var i = 0;
                for (var point = subborder.SortedPoints.First; point.Next != null; point = point.Next)
                {
                    array[i] = start + new Vector3(point.Value.X / 2f, point.Value.Y / 2f, -0.001f);
                    i++;
                }
                
                _lineRendererTST.positionCount = array.Length;
                _lineRendererTST.SetPositions(array);
            }
            
        }
    }
}