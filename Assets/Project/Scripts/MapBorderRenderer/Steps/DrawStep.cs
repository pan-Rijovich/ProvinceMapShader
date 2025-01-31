using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class DrawStep : IBorderCreationStep
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly bool _showExecutionInfo;
        private readonly MapBorderData _data;
        private readonly LineRenderer _linePrefab;
        private readonly Transform _container;
        private int _borderPartCounter = 0;
        private int _borderPointsCounter = 0;

        public DrawStep(MapBorderData data, LineRenderer linePrefab, Transform container, bool showExecutionInfo = false)
        {
            _data = data;
            _linePrefab = linePrefab;
            _container = container;
            _showExecutionInfo = showExecutionInfo;
        }


        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode){}
        
        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} created {_borderPartCounter} border objects";
            msg += $", which contained a total of {_borderPointsCounter} points";
            msg += $" in {_stopwatch.ElapsedMilliseconds} milliseconds";

            return msg;
        }
        
        public async Task Execute()
        {
            _stopwatch.Restart();

            foreach (var border in _data.Borders.Values)
            {
                foreach (var subBorder in border)
                {
                    foreach (var list in subBorder.SortedPointsLists)
                    {
                        DrawBorder(border.ID, subBorder, list);
                    }
                }
            }
            
            _stopwatch.Stop();
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            await Task.Yield();
        }
        
        private void DrawBorder(long id, SubBorder subBorder, LinkedList<BorderPoint> points)
        {
            _borderPartCounter++;
            var line = Object.Instantiate(_linePrefab, _container);
            line.name = $"Border ID:{id}   С1:{subBorder.ClusterIndexForColor1} С2:{subBorder.ClusterIndexForColor2} ";
            
            Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2);
            //Vector3 start = _data.MeshBounds.min;
            start.z = 0;
            Vector3[] linePoints = new Vector3[points.Count];
            
            var counter = 0;
            foreach (var point in points)
            {
                var normalizedPointX = point.X / 2f / _data.TextureWidth * _data.MeshSize.x;
                var normalizedPointY = point.Y / 2f / _data.TextureHeight * _data.MeshSize.y;
                linePoints[counter] = start + new Vector3(normalizedPointX, normalizedPointY, -0.002f);
                linePoints[counter].z = GetPointHeight(linePoints[counter]);
                counter++;
                _borderPointsCounter++;
            }

            line.positionCount = linePoints.Length * 2;
            line.SetPositions(BSpline.CalculateBSpline(linePoints.ToList(), line.positionCount).ToArray());
            //line.positionCount = linePoints.Length;
            //line.SetPositions(linePoints);
            
            line.loop = subBorder.IsCycled;
            line.gameObject.isStatic = true;
            subBorder.Lines.Add(line);
        }

        private float GetPointHeight(Vector3 point)
        {
            var height = -0.002f;

            if (Physics.Raycast(point, new Vector3(0f, 0f, 1f), out RaycastHit hit))
            {
                height = hit.point.z - 0.08f;
            }

            return height;
        }
    }
}