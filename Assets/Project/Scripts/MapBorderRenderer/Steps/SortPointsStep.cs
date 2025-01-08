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
        //private LinkedList<BorderPoint> _points;
        private BorderPoint _startPoint;
        private BorderPoint _currentPoint;
        private BorderPoint _previousPoint;
        private List<BorderPoint> _ = new();

        public SortPointsStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }
        
        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode)
        {
            var id = _data.GenerateBorderID(provColor.ToInt(), provColor2.ToInt());
            if (_data.Borders.TryGetValue(id, out var border))
            {
                Vector3 start = new Vector3(-_data.MeshSize.x / 2, -_data.MeshSize.y / 2);// + new Vector3(0.5f, 0.5f);
                
                
                foreach (var subborder in border)
                {

                    if (mode != 0)
                    {
                        foreach (var list in subborder.SortedPointsLists)
                        {
                            Gizmos.color = Color.green;
                            var edgePoint = list.First;
                            var position = start + new Vector3(edgePoint.Value.X / 2f, edgePoint.Value.Y / 2f);
                            Gizmos.DrawSphere(position, 0.25f);
                    
                            Gizmos.color = Color.magenta;
                            edgePoint = list.Last;
                            position = start + new Vector3(edgePoint.Value.X / 2f, edgePoint.Value.Y / 2f);
                            Gizmos.DrawSphere(position, 0.25f);
                            
                            if (mode == 2) break;
                        }
                        
                    }
                    
                    foreach (var list in subborder.SortedPointsLists)
                    {
                        var colorStep = 1f / list.Count;
                        var color = 1f;
                        
                        foreach (var point in list)
                        {
                            color -= colorStep;
                            Gizmos.color = new Color(color, 0f, 0f);
                        
                            Vector3 pos = start + new Vector3(point.X / 2f, point .Y / 2f);
                            Gizmos.DrawSphere(pos, 0.15f);
                        }
                        
                        
                    }
                    
                    if (mode == 2) break;
                }
            }
        }
        
        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} executed in {_stopwatch.ElapsedMilliseconds} milliseconds";
            return msg;
        }

        public async Task Execute()
        {
            _stopwatch.Restart();
            foreach (var border in _data.Borders.Values)
            {            
                foreach (var subBorder in border)
                {
                    _unsortedPoints = subBorder.UnsortedPoints;

                    while (subBorder.UnsortedPoints.Count > 0)
                    {
                        var list = new LinkedList<BorderPoint>();
                        subBorder.SortedPointsLists.Add(list);
                        
                        if (subBorder.UnsortedPoints.Count == 2)
                        {
                            _startPoint = _previousPoint;
                        }
                        if (subBorder.UnsortedPoints.Count  == 1)
                        {
                            _startPoint = _previousPoint;
                        }
                        
                        _startPoint = _previousPoint = _currentPoint = subBorder.UnsortedPoints.First();
                        list.AddFirst(_currentPoint);
                        _unsortedPoints.Remove(_currentPoint);
                        
                        //for (int i = 0; i < subBorder.UnsortedPoints.Count; i++)
                        while (subBorder.UnsortedPoints.Count > 0)
                        {
                            _currentPoint = GetMostPriorityNeighborPoint(_currentPoint);
                            if (_currentPoint == _previousPoint)
                            {
                                break;
                            }
                            list.AddFirst(_currentPoint);
                            _unsortedPoints.Remove(_currentPoint);
                            _previousPoint = _currentPoint;
                        }
                        
                        _currentPoint = GetMostPriorityNeighborPoint(_startPoint);
                        if (_currentPoint == _startPoint)
                        {
                            break;
                        }
                        list.AddLast(_currentPoint);
                        _unsortedPoints.Remove(_currentPoint);
                        
                        //for (int i = 0; i < subBorder.UnsortedPoints.Count; i++)
                        while (subBorder.UnsortedPoints.Count > 0)
                        {
                            _currentPoint = GetMostPriorityNeighborPoint(_currentPoint);
                            if (_currentPoint == _previousPoint)
                            {
                                break;
                            }
                            list.AddLast(_currentPoint);
                            _unsortedPoints.Remove(_currentPoint);
                            _previousPoint = _currentPoint;
                        }
                        
                        
                        //await Task.Yield();
                    }
                    
                    //check on cycle border condition
                    CheckSubBorderCycling(subBorder);
                }
            }
            
            
            _stopwatch.Stop();
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());

            await Task.Yield();
        }
        
        public void CheckSubBorderCycling(SubBorder subBorder)
        {
            if(subBorder.SortedPointsLists.Count > 1) return;
            
            var startPoint = subBorder.SortedPointsLists[0].First();
            var endPoint = subBorder.SortedPointsLists[0].Last();
            
            if (startPoint.IsEdgePoint == false)
            {
                if (startPoint + new Vector2Int(1, 0) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(0, 1) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(-1, 0) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(0, -1) == endPoint) subBorder.IsCycled = true;
                
                
                if (startPoint + new Vector2Int(1, 1) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(1, -1) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(-1, 1) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(-1, -1) == endPoint) subBorder.IsCycled = true;

                if (startPoint + new Vector2Int(2, 0) == endPoint)
                {
                    if(CalculateParentOffset(startPoint, endPoint) == 1) subBorder.IsCycled = true;
                }
                if (startPoint + new Vector2Int(0, 2) == endPoint) 
                {
                    if(CalculateParentOffset(startPoint, endPoint) == 1) subBorder.IsCycled = true;
                }
                if (startPoint + new Vector2Int(-2, 0) == endPoint) 
                {
                    if(CalculateParentOffset(startPoint, endPoint) == 1) subBorder.IsCycled = true;
                }
                if (startPoint + new Vector2Int(0, -2) == endPoint) 
                {
                    if(CalculateParentOffset(startPoint, endPoint) == 1) subBorder.IsCycled = true;
                }
            }
            else
            {
                if (startPoint + new Vector2Int(1, 0) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(0, 1) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(-1, 0) == endPoint) subBorder.IsCycled = true;
                if (startPoint + new Vector2Int(0, -1) == endPoint) subBorder.IsCycled = true;
            }
        }

        private BorderPoint GetMostPriorityNeighborPoint(BorderPoint currentPoint)
        {
            BorderPoint result;
            if (currentPoint.IsEdgePoint == false)
            {
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(1, 0), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, 1), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(-1, 0), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, -1), out result)) return result;
                
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(1, 1), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(1, -1), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(-1, 1), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(-1, -1), out result)) return result;

                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(2, 0), out result))
                {
                    if(CalculateParentOffset(currentPoint, result) == 1) return result;
                    
                }
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, 2), out result)) 
                {
                    if(CalculateParentOffset(currentPoint, result) == 1) return result;
                    
                }
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(-2, 0), out result)) 
                {
                    if(CalculateParentOffset(currentPoint, result) == 1) return result;
                    
                }
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, -2), out result)) 
                {
                    if(CalculateParentOffset(currentPoint, result) == 1) return result;
                }
            }
            else
            {
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(1, 0), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, 1), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(-1, 0), out result)) return result;
                if (_unsortedPoints.TryGetValue(currentPoint + new Vector2Int(0, -1), out result)) return result;
            }
            result = currentPoint;
            return result;
        }

        private int CalculateParentOffset(BorderPoint currentPoint, BorderPoint offsetPoint)
        {
            var currentFrom = _data.ConvertIndexToIntPixelCoordinated(currentPoint.FromPixelIndex);
            var currentTo = _data.ConvertIndexToIntPixelCoordinated(currentPoint.ToPixelIndex);
            var offsetFrom = _data.ConvertIndexToIntPixelCoordinated(offsetPoint.FromPixelIndex);
            var offsetTo = _data.ConvertIndexToIntPixelCoordinated(offsetPoint.ToPixelIndex);
            
            var xDifference = currentFrom.x - offsetFrom.x;
            var yDifference = currentTo.y - offsetTo.y;
            
            return Mathf.Max(Mathf.Abs(xDifference), Mathf.Abs(yDifference));
        }
    }
}