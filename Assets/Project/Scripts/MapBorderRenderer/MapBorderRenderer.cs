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

        private List<IBorderCreationStep> _steps = new(10);
        private MapBorderData _data;
        
        private Color32 _debugProvince;

        private async void Start()
        {
            _data = new(GetComponent<Renderer>().material, GetComponent<MeshFilter>().mesh, transform);

            _steps = new()
            {
                new BorderPixelsCollectionStep(_data, _showExecutionInfo),
                //new PointsCreationStep(_data, _showExecutionInfo),
                //new PointsClearStep(_data),
                //new WriteBordersInPovinceStep(_data),
                //new SortPointsStep(_data),
                //new FilterStraightPointsStep(_data),
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
            var step = _steps[0] as BorderPixelsCollectionStep;
            step.DrawGizmos(_debugProvince);
            
        }

        private void Update()
        {
            var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var p = hitInfo.textureCoord;
                int x = (int)Mathf.Floor(p.x * _data.TextureWidth);
                int y = (int)Mathf.Floor(p.y * _data.TextureHeight);

                _debugProvince = _data.Texture.GetPixel(x, y);
            }
        }
    }
}