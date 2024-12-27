using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class FilterStraightPointsStep : IBorderCreationStep
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly bool _showExecutionInfo;
        private MapBorderData _data;
        private HashSet<int> _pointsForDelete = new (100);
        private int _deletedPointsCount;
        private BorderPoint _currentPoint;
        private BorderPoint _previousPoint;
        private Vector2Int _moveDirection;

        public FilterStraightPointsStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }

        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode) { }

        public async Task Execute()
        {
            _stopwatch.Restart();

            _deletedPointsCount = 0;
            foreach (var border in _data.Borders.Values)
            {            
                foreach (var subBorder in border)
                {
                    _pointsForDelete.Clear();
                    var counter = -1;
                    
                    for (var node = subBorder.SortedPoints.First; node.Next != null; node = node.Next)
                    {
                        counter++;
                        if(node.Previous == null) continue;
                        if(node.Next == null) continue;
                        
                        var previousDirection = GetMoveDirection(node.Previous.Value, node.Value);
                        var nextDirection = GetMoveDirection(node.Value, node.Next.Value);

                        if (previousDirection == nextDirection)
                        {
                            _pointsForDelete.Add(counter);
                        }
                        
                    }
                    
                    counter = -1;
                    LinkedListNode<BorderPoint> toRemove = null;
                    
                    for (var node = subBorder.SortedPoints.First; node.Next != null; node = node.Next)
                    {
                        counter++;
                        
                        if (_pointsForDelete.Contains(counter))
                        {
                            if(toRemove != null) subBorder.SortedPoints.Remove(toRemove);
                            toRemove = node;
                            
                        }
                        
                    }
                }
                
                _deletedPointsCount += _pointsForDelete.Count;
            }
            
            _stopwatch.Stop();
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            await Task.Yield();
        }
        
        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} deleted {_deletedPointsCount} points in {_stopwatch.ElapsedMilliseconds} milliseconds";
            return msg;
        }


        private Vector2Int GetMoveDirection(BorderPoint from, BorderPoint to)
        {
            return new (from.X - to.X, from.Y - to.Y);
        }
        

        private enum MoveDirection
        {
            None,
            Up, Down, Left, Right, 
            UpLeft, UpRight, DownRight, DownLeft
        }
    }
}