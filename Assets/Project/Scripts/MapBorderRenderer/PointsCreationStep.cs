using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MapBorderRenderer
{
    public class PointsCreationStep : IBorderCreationStep
    {
        private MapBorderData _data;

        public PointsCreationStep(MapBorderData data)
        {
            _data = data;
        }

        public async Task Execute()
        {
            var width = _data.TextureWidth;
            var height = _data.TextureHeight;
            var provinceArr = _data.TextureArr;
            for (int i = 0; i < provinceArr.Length - 1; i++)
            {
                Color32 current = provinceArr[i];
                Color32 right = provinceArr[i + 1];
                Color32 up = provinceArr[i];
                if (i + _data.TextureWidth < provinceArr.Length)
                    up = provinceArr[i + _data.TextureWidth];

                if (!current.CompareRGB(up))
                {
                    long id = GenerateBorderID(current, up);
                    var point = new BorderPoint(i, width, height);
                    if (_data.Borders.ContainsKey(id) == false) _data.Borders[id] = new(id);
                    var border = _data.Borders[id];
                    point.DebugColor = 2;

                    border[0].Add(point);
                    point.X -= 2;
                    border[0].Add(point);
                }

                if (!current.CompareRGB(right))
                {
                    long id = GenerateBorderID(current, right);
                    var point = new BorderPoint(i, width, height);
                    if (_data.Borders.ContainsKey(id) == false) _data.Borders[id] = new(id);
                    var border = _data.Borders[id];
                    point.DebugColor = 1;

                    border[0].Add(point);
                    point.Y -= 2;
                    border[0].Add(point);
                }
            }

            await Task.Delay(1);
        }

        private long GenerateBorderID(Color32 color1, Color32 color2)
        {
            long id;
            if (color1.LessThanColor(color2))
            {
                id = ((long)color2.Color32ToUInt() << 32) + color1.Color32ToUInt();
            }
            else
            {
                id = ((long)color1.Color32ToUInt() << 32) + color2.Color32ToUInt();
            }
            return id;
        }
    }
}