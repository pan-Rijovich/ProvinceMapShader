using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace MapBorderRenderer
{
    public class SortPointsStep : IBorderCreationStep
    {
        private MapBorderData _data;
        private List<BorderPoint> _unsortedPoints;
        private BorderPoint _currentPoint;

        public SortPointsStep(MapBorderData data)
        {
            _data = data;
        }

        public async Task Execute()
        {
            foreach (var border in _data.Borders.Values)
            {
                if (border.Points[0].Count == 1) Debug.LogError("Detected one point border");
                if (border.Points[0].Count < 3) continue;
                _unsortedPoints = new List<BorderPoint>(border.Points[0]);
                _currentPoint = _unsortedPoints[0];
                var borderNumber = 0;
                _unsortedPoints.RemoveAt(0);
                border.Points.Clear();
                border.Points.Add(new());
                border.Points[borderNumber].Add(_currentPoint);

                while (_unsortedPoints.Count != 0)
                {
                    var neighbours = GetNeighbours();

                    if (neighbours.Count == 0)
                    {
                        //borderNumber++;
                    }
                    else if(neighbours.Count == 1)
                    {

                    }
                }
            }
        }

        private List<BorderPoint> GetNeighbours()
        {
            var list = new List<BorderPoint>();

            var leftNeighbour = new BorderPoint() { X = _currentPoint.X - 2, Y = _currentPoint.Y };
            var rightNeighbour = new BorderPoint() { X = _currentPoint.X + 2, Y = _currentPoint.Y };
            var topNeighbour = new BorderPoint() { X = _currentPoint.X, Y = _currentPoint.Y + 2 };
            var downNeighbour = new BorderPoint() { X = _currentPoint.X, Y = _currentPoint.Y - 2 };

            var hasLeftNeighbour = _unsortedPoints.Contains(leftNeighbour);
            var hasRightNeighbour = _unsortedPoints.Contains(rightNeighbour);
            var hasTopNeighbour = _unsortedPoints.Contains(topNeighbour);
            var hasDownNeighbour = _unsortedPoints.Contains(downNeighbour);

            if (hasLeftNeighbour) list.Add(leftNeighbour);
            if (hasRightNeighbour) list.Add(rightNeighbour);
            if (hasTopNeighbour) list.Add(topNeighbour);
            if (hasDownNeighbour) list.Add(downNeighbour);

            return list;
        }
    }
}