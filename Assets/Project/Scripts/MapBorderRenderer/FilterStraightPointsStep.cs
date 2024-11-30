using System.Collections.Generic;
using System.Linq;

namespace MapBorderRenderer
{
    public class FilterStraightPointsStep
    {
        private MapBorderData _data;

        public FilterStraightPointsStep(MapBorderData data)
        {
            _data = data;
        }

        public void Execute()
        {
/*            var width = _data._provinceTexWidth;

            foreach (var br in _data.BordersFloat.Values)
            {
                foreach (var b in br)
                {
                    var border = new List<Float2>();

                    for (int i = 1; i < b.Count - 1; i++)
                    {
                        int posUp = b[i].positionIndex + width;
                        int posDown = b[i].positionIndex - width;
                        int posLeft = b[i].positionIndex - 1;
                        int posRight = b[i].positionIndex + 1;
                        var prev = b[i - 1].positionIndex;
                        var next = b[i + 1].positionIndex;
                        bool IsHorisontal = prev == posLeft && next == posRight || prev == posRight && next == posLeft;
                        bool IsVertical = prev == posDown && next == posUp || prev == posUp && next == posDown;
                        bool IsInnerCorner = true;
                        bool ISOuterCorner = prev == posDown && next == posRight || prev == posRight && next == posDown;

                        if (!IsHorisontal && !IsVertical)
                        {
                            border.Add(b[i]);
                        }
                    }

                    border.Insert(0, b[0]);
                    border.Add(b.Last());

                    b.Clear();
                    b.AddRange(border);
                }
            }*/
        }
    }
}