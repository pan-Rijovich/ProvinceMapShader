using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class SortPointsStep : IBorderCreationStep
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly bool _showExecutionInfo;
        private readonly MapBorderData _data;
        private HashSet<BorderPoint> _unsortedPoints;
        private LinkedList<BorderPoint> _points;
        private BorderPoint _startPoint;
        private BorderPoint _currentPoint;
        private BorderPoint _previousPoint;
        private List<BorderPoint> _zeroStartPoints = new();

        public SortPointsStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }
        
        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode)
        {
            var id = _data.GenerateBorderID(_data.Color32ToInt(provColor), _data.Color32ToInt(provColor2));
            if (_data.Borders.TryGetValue(id, out var border))
            {
                Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2);// + new Vector3(0.5f, 0.5f);
                
                foreach (var point in _zeroStartPoints)
                {
                    Gizmos.color = new Color(0f, 0.5f, 1f);
                        
                    Vector3 pos = start + new Vector3(point.X / 2f, point .Y / 2f);
                    Gizmos.DrawSphere(pos, 0.45f);
                                             
                                         
                }
                
                foreach (var subborder in border)
                {
                    var colorStep = 1f / subborder.SortedPoints.Count;
                    var color = 1f;
                    
                    foreach (var point in subborder.SortedPoints)
                    {
                        color -= colorStep;
                        Gizmos.color = new Color(color, 0f, 0f);
                        
                        Vector3 pos = start + new Vector3(point.X / 2f, point .Y / 2f);
                        Gizmos.DrawSphere(pos, 0.15f);
                    }

                    
                    Gizmos.color = Color.green;
                    var edgePoint = subborder.SortedPoints.First;
                    var position = start + new Vector3(edgePoint.Value.X / 2f, edgePoint.Value.Y / 2f);
                    Gizmos.DrawSphere(position, 0.25f);
                    
                    Gizmos.color = Color.magenta;
                    edgePoint = subborder.SortedPoints.Last;
                    position = start + new Vector3(edgePoint.Value.X / 2f, edgePoint.Value.Y / 2f);
                    Gizmos.DrawSphere(position, 0.25f);
                }
            }
        }

        public async Task Execute()
        {
            _stopwatch.Restart();
            foreach (var border in _data.Borders.Values)
            {            
                foreach (var subBorder in border)
                {
                    _points = subBorder.SortedPoints;
                    _unsortedPoints = subBorder.UnsortedPoints;
                    _startPoint = _previousPoint = _currentPoint = subBorder.UnsortedPoints.First();
                    _points.AddLast(_currentPoint);


                    var neighbors = GetNeighbors(_startPoint);
                    //if(neighbors.Count > 2) Debug.LogError($"Point has {neighbors.Count} neighbors");


                    if (neighbors.Count == 0)
                    {
                        _zeroStartPoints.Add(_startPoint);
                        Debug.LogError($"Zero Start Point in X {_startPoint.X}     Y {_startPoint.Y}");
                        continue;
                    }
                    
                    _currentPoint = neighbors[0];
                    
                    for (int i = 0; i < subBorder.UnsortedPoints.Count; i++)
                    {
                        if (TryAddPointAsLast(new Vector2Int(1, 1))) continue;
                        if (TryAddPointAsLast(new Vector2Int(1, -1))) continue;
                        if (TryAddPointAsLast(new Vector2Int(-1, 1))) continue;
                        if (TryAddPointAsLast(new Vector2Int(-1, -1))) continue;

                        if (TryAddPointAsLast(new Vector2Int(2, 0))) continue;
                        if (TryAddPointAsLast(new Vector2Int(0, 2))) continue;
                        if (TryAddPointAsLast(new Vector2Int(-2, 0))) continue;
                        if (TryAddPointAsLast(new Vector2Int(0, -2))) continue;

                        if (TryAddPointAsLast(new Vector2Int(1, 0))) continue;
                        if (TryAddPointAsLast(new Vector2Int(0, 1))) continue;
                        if (TryAddPointAsLast(new Vector2Int(-1, 0))) continue;
                        if (TryAddPointAsLast(new Vector2Int(0, -1))) continue;
                    }


                    if (neighbors.Count > 1)
                    {
                        _currentPoint = neighbors[1];
                        _previousPoint = _startPoint;
                    
                        for (int i = 0; i < subBorder.UnsortedPoints.Count; i++)
                        {
                            if (TryAddPointAsFirst(new Vector2Int(1, 1))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(1, -1))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(-1, 1))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(-1, -1))) continue;

                            if (TryAddPointAsFirst(new Vector2Int(2, 0))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(0, 2))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(-2, 0))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(0, -2))) continue;

                            if (TryAddPointAsFirst(new Vector2Int(1, 0))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(0, 1))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(-1, 0))) continue;
                            if (TryAddPointAsFirst(new Vector2Int(0, -1))) continue;
                        }
                    }
                    
                    
                }
            }
            
            
            _stopwatch.Stop();
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());

            await Task.Yield();
        }
        
        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} executed in {_stopwatch.ElapsedMilliseconds} milliseconds";
            return msg;
        }

        private List<BorderPoint> GetNeighbors(BorderPoint startPoint)
        {
            var neighbors = new List<BorderPoint>();
            
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(1, 1))) neighbors.Add(startPoint + new Vector2Int(1, 1));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(1, -1))) neighbors.Add(startPoint + new Vector2Int(1, -1));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(-1, 1))) neighbors.Add(startPoint + new Vector2Int(-1, 1));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(-1, -1))) neighbors.Add(startPoint + new Vector2Int(-1, -1));

            if (_unsortedPoints.Contains(startPoint + new Vector2Int(2, 0))) neighbors.Add(startPoint + new Vector2Int(2, 0));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(0, 2))) neighbors.Add(startPoint + new Vector2Int(0, 2));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(-2, 0))) neighbors.Add(startPoint + new Vector2Int(-2, 0));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(0, -2))) neighbors.Add(startPoint + new Vector2Int(0, -2));

            if (_unsortedPoints.Contains(startPoint + new Vector2Int(1, 0))) neighbors.Add(startPoint + new Vector2Int(1, 1));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(0, 1))) neighbors.Add(startPoint + new Vector2Int(1, 1));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(-1, 0))) neighbors.Add(startPoint + new Vector2Int(-1, 0));
            if (_unsortedPoints.Contains(startPoint + new Vector2Int(0, -1))) neighbors.Add(startPoint + new Vector2Int(-1, 0));

            return neighbors;
        }
        
        private bool TryAddPointAsLast(Vector2Int offset)
        {
            if (_currentPoint + offset != _previousPoint && _unsortedPoints.TryGetValue(_currentPoint + offset, out var point))
            {
                _points.AddLast(point);
                _previousPoint = _currentPoint;
                _currentPoint = point;
                return true;
            }
            return false;
        }
        
        private bool TryAddPointAsFirst(Vector2Int offset)
        {
            if (_currentPoint + offset != _previousPoint && _unsortedPoints.TryGetValue(_currentPoint + offset, out var point))
            {
                _points.AddFirst(point);
                _previousPoint = _currentPoint;
                _currentPoint = point;
                return true;
            }
            return false;
        }
    }
}