using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MapBorderRenderer
{
    public class DrawLinesStep : IBorderCreationStep
    {
        private Transform _linesContainer;
        private LineRenderer _prefab;
        private MapBorderData _data;

        public DrawLinesStep(MapBorderData data, LineRenderer prefab)
        {
            _data = data;
            _prefab = prefab;
        }

        public async Task Execute()
        {
            await Task.Delay(300);


        }

        private void DrawLines(uint color)
        {
            foreach (var borderID in _data.ProvincesBorders[color])
            {
                foreach (var border in _data.Borders[borderID].Points)
                {
                    _data._lineRenderer.Add(Object.Instantiate(_prefab, _linesContainer));
                    Vector3[] points = new Vector3[border.Count];
                    for (int i = 0; i < border.Count; i++)
                    {
                        points[i] = CalcPosFromUV(border[i], _data.Transform.position) + new Vector3(0, 0, -0.01f);
                        
                    }

                    _data._lineRenderer.Last().positionCount = points.Length;
                    //_data._lineRenderer.Last().SetPositions(BSpline.CalculateBSpline(points.ToList(), 50).ToArray());
                    _data._lineRenderer.Last().SetPositions(points);
                }
            }
        }

        private Vector3 CalcPosFromUV(BorderPoint i, Vector3 meshCenter)
        {

            meshCenter.x -= _data.MeshSize.x / 2;
            meshCenter.y -= _data.MeshSize.y / 2;

            meshCenter.x += i.X * _data.MeshSize.x;
            meshCenter.y += i.Y * _data.MeshSize.y;

            return meshCenter;
        }

        private void LineRendererBorderSmooth(uint color)
        {
            if (_data._lineRenderer.Count == 0)
            {
                DrawLines(color);
            }
            else
            {
                foreach (var line in _data._lineRenderer)
                {
                    line.gameObject.SetActive(false);
                }
                _data._lineRenderer.Clear();
                DrawLines(color);
            }
        }
    }
}