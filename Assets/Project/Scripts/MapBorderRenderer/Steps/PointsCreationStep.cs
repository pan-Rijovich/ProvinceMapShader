using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class PointsCreationStep : IBorderCreationStep
    {
        private MapBorderGenData _genData;
        private Stopwatch _stopwatch = new();
        private bool _showExecutionInfo;

        private int _fromPixelIndex;
        private int _toPixelIndex;
        private int _fromColor;
        private int _toColor;
        private int _fromClusterIndex;
        private int _toClusterIndex;

        public PointsCreationStep(MapBorderGenData genData, bool showExecutionInfo = false)
        {
            _genData = genData;
            _showExecutionInfo = showExecutionInfo;
        }
        
        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode)
        {
            var id = _genData.GenerateBorderID(provColor.ToInt(), provColor2.ToInt());
            if (_genData.BordersCreationData.TryGetValue(id, out var border))
            {
                Vector3 start = new Vector3(-_genData.MapSize.x / 2, -_genData.MapSize.y / 2);// + new Vector3(0.5f, 0.5f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(start, 0.15f);
                Gizmos.DrawSphere(start + new Vector3(1f, 1f), 0.15f);
                
                
                
                foreach (var subborder in border)
                {
                    if(mode != 0) Gizmos.color = new Color(Random.value, Random.value, Random.value);
                    //Gizmos.color = Color.red;
                    foreach (var point in subborder)
                    {
                        if (mode == 0)
                        {
                            if(point.DebugColor == 0) Gizmos.color = Color.white;
                            if(point.DebugColor == 1) Gizmos.color = Color.black;
                            if(point.DebugColor == 2) Gizmos.color = Color.red;
                            if(point.DebugColor == 3) Gizmos.color = Color.green;
                            if(point.DebugColor == 4) Gizmos.color = new Color(1f, 0.5f, 0f);
                        }
                        Vector3 pos = start + new Vector3(point.X / 2f, point .Y / 2f);
                        Gizmos.DrawSphere(pos, 0.15f);
                    }
                }
            }
        }

        public async Task Execute()
        {
            _stopwatch.Restart();
            foreach (var pair in _genData.BorderPixels)
            {
                foreach (var cluster in pair.Value)
                {
                    foreach (var pixel in cluster)
                    {
                        TryGeneratePointBetweenPixels(pixel, _genData.GetUpIndex(pixel), MoveDirection.Up);
                        TryGeneratePointBetweenPixels(pixel, _genData.GetDownIndex(pixel), MoveDirection.Down);
                        TryGeneratePointBetweenPixels(pixel, _genData.GetLeftIndex(pixel), MoveDirection.Left);
                        TryGeneratePointBetweenPixels(pixel, _genData.GetRightIndex(pixel), MoveDirection.Right);
                    }
                }
            }
            _stopwatch.Stop();
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            
            _genData.TexPixels = null;
            //_data.BorderPixels  = null;

            await Task.Yield();
        }
        
        public string GetExecutionInfo()
        {
            int bordersCount = _genData.BordersCreationData.Count;
            int subBordersCount = 0;
            int pointsCount = 0;

            foreach (var border in _genData.BordersCreationData.Values)
            {
                foreach (var subBorder in border)
                {
                    subBordersCount++;
                    foreach (var point in subBorder)
                    {
                        pointsCount++;
                    }
                }
            }
            
            var msg = $"{GetType().Name} created {bordersCount} borders,";
            msg += $" {subBordersCount} sub-borders";
            msg += $" and {pointsCount} points";
            msg += $" in {_stopwatch.ElapsedMilliseconds} milliseconds";
            return msg;
        }

        private void TryGeneratePointBetweenPixels(int fromPixelIndex, int toPixelIndex,  MoveDirection checkDirection)
        {
            _fromPixelIndex = fromPixelIndex;
            _toPixelIndex = toPixelIndex;
            _fromColor = _genData.TexPixels[_fromPixelIndex].ToInt();
            _toColor = _genData.TexPixels[_toPixelIndex].ToInt();
            if (_fromColor == _toColor) return;
            
            
            var id = _genData.GenerateBorderID(_fromColor, _toColor);
            var border = GetBorder(id, _fromColor, _toColor);
                
            _genData.BorderPixels[_fromColor].TryGetClusterNumberForPixelIndex(_fromPixelIndex, out _fromClusterIndex);
            _genData.BorderPixels[_toColor].TryGetClusterNumberForPixelIndex(_toPixelIndex, out _toClusterIndex);

            if (_fromClusterIndex == -1 || _toClusterIndex == -1)
            {
                Debug.LogError("Try to generate point between pixels, that can not get cluster numbers");
                return;
            }

            var subBorder = GetSubBorder(border, _fromColor, _toColor,_fromClusterIndex, _toClusterIndex);
            var point = GetBorderPoint(_fromPixelIndex, _toPixelIndex);
            subBorder.AddPoint(point);


            if (checkDirection == MoveDirection.Up || checkDirection == MoveDirection.Down)
            {
                TryAddHorizontalEdgePoints(point, subBorder);
            }
            else
            {
                TryAddVerticalEdgePoints(_fromPixelIndex, _toPixelIndex, _fromColor, _toColor, point, subBorder);
            }
        }

        private BorderCreationData GetBorder(long id, int color1, int color2)
        {
            if (_genData.BordersCreationData.TryGetValue(id, out var createBorder)) return createBorder;
            
            var border = new BorderCreationData(id, color1, color2);
            _genData.BordersCreationData.Add(id, border);
                
            return border;
        }

        private SubBorderCreationData GetSubBorder(BorderCreationData border, int color1, int color2, int color1ClusterIndex, int color2ClusterIndex)
        {
            if (border.Color1 == color2)
            {
                var tmp = color1ClusterIndex;
                color1ClusterIndex = color2ClusterIndex;
                color2ClusterIndex = tmp;
            }
            
            if (border.TryGetSubBorder(color1ClusterIndex, color2ClusterIndex, out var subBorder))
            {
                return subBorder;
            }
            
            subBorder = border.AddSubBorder(color1ClusterIndex, color2ClusterIndex);

            return subBorder;
        }

        private BorderPoint GetBorderPoint(int fromIndex, int toIndex)
        {
            var point = new BorderPoint();
            var difference = toIndex - fromIndex;
            var width = _genData.TexWidth;
            
            point.X = (fromIndex % width);
            point.Y = (fromIndex / width);
            point.X = (point.X) * 2 + 1;
            point.Y = (point.Y) * 2 + 1;
            point.FromPixelIndex = fromIndex;
            point.ToPixelIndex = toIndex;
            
            if(difference == width) //up
            {
                point.Y += 1;
                point.DebugColor = 0;
            }
            else if(difference == -width) //down
            {
                point.Y -=  1;
                point.DebugColor = 1;
            }
            else if(difference == 1) //left
            {
                point.X +=  1;
                point.DebugColor = 2;
            }
            else if(difference == -1) //right
            {
                point.X -=  1;
                point.DebugColor = 3;
            }

            return point;
        }

        private void TryAddHorizontalEdgePoints(BorderPoint point, SubBorderCreationData subBorder)
        {
            var neighborPixelIndex = _genData.GetLeftIndex(_fromPixelIndex);
            var neighborColor = _genData.TexPixels[neighborPixelIndex].ToInt();
            var neighborComparision = GetEdgeColorComparision(neighborColor, neighborPixelIndex);
            
            var diagonalNeighborPixelIndex = _genData.GetLeftIndex(_toPixelIndex);
            var diagonalNeighborColor = _genData.TexPixels[diagonalNeighborPixelIndex].ToInt();
            var diagonalNeighborComparision = GetEdgeColorComparision(diagonalNeighborColor, diagonalNeighborPixelIndex);
                
            //neighborComparision = _fromColor  != neighborColor && _toColor  != neighborColor;
            //diagonalNeighborComparision = _fromColor  != diagonalNeighborColor && _toColor  != diagonalNeighborColor;
            
            if (neighborComparision || diagonalNeighborComparision)
            {
                var edgePoint = point + Vector2Int.left;
                edgePoint.IsEdgePoint = true;
                edgePoint.DebugColor = 4;
                subBorder.AddPoint(edgePoint);
            }
                
            
            neighborPixelIndex = _genData.GetRightIndex(_fromPixelIndex);
            neighborColor = _genData.TexPixels[neighborPixelIndex].ToInt();
            neighborComparision = GetEdgeColorComparision(neighborColor, neighborPixelIndex);
            
            diagonalNeighborPixelIndex = _genData.GetRightIndex(_toPixelIndex);
            diagonalNeighborColor = _genData.TexPixels[diagonalNeighborPixelIndex].ToInt();
            diagonalNeighborComparision = GetEdgeColorComparision(diagonalNeighborColor, diagonalNeighborPixelIndex);
                
            if (neighborComparision || diagonalNeighborComparision)
            {
                var edgePoint = point + Vector2Int.right;
                edgePoint.IsEdgePoint = true;
                edgePoint.DebugColor = 4;
                subBorder.AddPoint(edgePoint);
            }
        }

        private void TryAddVerticalEdgePoints(int fromPixelIndex, int toPixelIndex, int fromColor, int toColor,
            BorderPoint point, SubBorderCreationData subBorder)
        {
            var neighborPixelIndex = _genData.GetUpIndex(_fromPixelIndex);
            var neighborColor = _genData.TexPixels[neighborPixelIndex].ToInt();
            var neighborComparision = GetEdgeColorComparision(neighborColor, neighborPixelIndex);
            
            var diagonalNeighborPixelIndex = _genData.GetUpIndex(_toPixelIndex);
            var diagonalNeighborColor = _genData.TexPixels[diagonalNeighborPixelIndex].ToInt();
            var diagonalNeighborComparision = GetEdgeColorComparision(diagonalNeighborColor, diagonalNeighborPixelIndex);
            
            if (neighborComparision || diagonalNeighborComparision)
            {
                var edgePoint = point + Vector2Int.up;
                edgePoint.IsEdgePoint = true;
                edgePoint.DebugColor = 4;
                subBorder.AddPoint(edgePoint);
            }
                
            neighborPixelIndex = _genData.GetDownIndex(_fromPixelIndex);
            neighborColor = _genData.TexPixels[neighborPixelIndex].ToInt();
            neighborComparision = GetEdgeColorComparision(neighborColor, neighborPixelIndex);
            
            diagonalNeighborPixelIndex = _genData.GetDownIndex(_toPixelIndex);
            diagonalNeighborColor = _genData.TexPixels[diagonalNeighborPixelIndex].ToInt();
            diagonalNeighborComparision = GetEdgeColorComparision(diagonalNeighborColor, diagonalNeighborPixelIndex);
                
            if (neighborComparision || diagonalNeighborComparision)
            {
                var edgePoint = point + Vector2Int.down;
                edgePoint.IsEdgePoint = true;
                edgePoint.DebugColor = 4;
                subBorder.AddPoint(edgePoint);
            }
        }

        private bool GetEdgeColorComparision(int neighborColor, int neighborPixelIndex)
        {
            if (_fromColor != neighborColor && _toColor != neighborColor)
            {
                return true;
            }
            else
            {
                _genData.BorderPixels[neighborColor].TryGetClusterNumberForPixelIndex(neighborPixelIndex, out var neighborClusterIndex);

                if (neighborClusterIndex == -1) return false;
                
                
                if (_fromColor == neighborColor && _fromClusterIndex != neighborClusterIndex)
                {
                    return true;
                }
                
                if ( _toColor == neighborColor && _toClusterIndex != neighborClusterIndex)
                {
                    return true;
                }
            }
            return false;
        }
        
        private enum MoveDirection
        {
            None,
            Up, Down, Left, Right, 
            UpLeft, UpRight, DownRight, DownLeft
        }
    }
}