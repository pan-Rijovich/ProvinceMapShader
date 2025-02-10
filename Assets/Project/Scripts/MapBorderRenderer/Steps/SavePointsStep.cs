using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Services.Storage;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class SavePointsStep : IBorderCreationStep
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly MapBorderData _data;
        private readonly bool _showExecutionInfo;
        private float _executionTime;

        private IStorageService _storage = new BinaryStorageService();

        public SavePointsStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }

        public async Task Execute()
        {
            _stopwatch.Restart();

            _data.BordersSaveData = new BorderSaveData[_data.BordersCreationData.Values.Count];

            var borderIndex = 0;
            foreach (var border in _data.BordersCreationData.Values)
            {
                var borderSave = new BorderSaveData();
                borderSave.ID = border.ID;
                borderSave.SubBorders = new SubBorderSaveData[border.SubBorders.Count];
                _data.BordersSaveData[borderIndex] = borderSave;

                var subborderIndex = 0;
                
                foreach (var subborder in border)
                {
                    var subborderSave = new SubBorderSaveData();
                    subborderSave.IsCycled = subborder.IsCycled;
                    subborderSave.ClusterIndexForColor1 = subborder.ClusterIndexForColor1;
                    subborderSave.ClusterIndexForColor2 = subborder.ClusterIndexForColor2;
                    subborderSave.SortedPointsLists = subborder.SortedPointsLists.Select(linkedList => 
                        linkedList.SelectMany(p => new[] { p.X, p.Y }).ToArray()).ToArray();
                    
                    
                    borderSave.SubBorders[subborderIndex] = subborderSave;
                    
                    subborderIndex++;
                }

                borderIndex++;
            }
            
            _storage.Save("BorderPoints", _data.BordersSaveData);
            
            _stopwatch.Stop();
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            await Task.Yield();
        }

        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} saved all border points in {_stopwatch.ElapsedMilliseconds} milliseconds";

            return msg;
        }

        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode){}
    }
}