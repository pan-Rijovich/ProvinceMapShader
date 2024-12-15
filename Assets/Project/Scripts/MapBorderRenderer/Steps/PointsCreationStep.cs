using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MapBorderRenderer
{
    public class PointsCreationStep : IBorderCreationStep
    {
        private MapBorderData _data;
        private bool _showExecutionInfo;

        public PointsCreationStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }

        public async Task Execute()
        {
            foreach (var pair in _data.BorderPixels)
            {
                var color = pair.Key;
                var clusters = pair.Value;

                foreach (var cluster in clusters)
                {
                    foreach (var pixel in cluster)
                    {
                        var pixelColor = _data.TextureArr[pixel];
                        

                    }
                }
            }
            
            
            
            
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());

            await Task.Delay(1);
        }
        
        public string GetExecutionInfo()
        {
            var msg = $"";
            
            return msg;
        }

        private void TryGeneratePointBetweenPixels(int fromIndex, int toIndex)
        {
            var fromColor = _data.TextureArr[fromIndex];
            var toColor = _data.TextureArr[toIndex];
            
            if (fromColor.CompareRGB(toColor) == false)
            {
                var id = _data.GenerateBorderID(fromColor, toColor);
                if(_data.Borders.ContainsKey(id) == false) _data.Borders.Add(id, new Border(id));
                var border = _data.Borders[id];
                
            }
        }

    }
}