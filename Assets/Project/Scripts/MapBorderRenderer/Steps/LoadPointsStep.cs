using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Services.Storage;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapBorderRenderer
{
    public class LoadPointsStep : IBorderCreationStep
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly MapBorderGenData _genData;
        private readonly bool _showExecutionInfo;
        private float _executionTime;

        private IStorageService _storage = new BinaryStorageService();

        public LoadPointsStep(MapBorderGenData genData, bool showExecutionInfo = false)
        {
            _genData = genData;
            _showExecutionInfo = showExecutionInfo;
        }

        public async Task Execute()
        {
            _stopwatch.Restart();
            
            _storage.Load<BorderSaveData[]>("BorderPoints", 
                obj => _genData.BordersSaveData = obj);
            
            _stopwatch.Stop();
            if(_showExecutionInfo) Debug.Log(GetExecutionInfo());
            await Task.Yield();
        }

        public string GetExecutionInfo()
        {
            var msg = $"{GetType().Name} load all border points in {_stopwatch.ElapsedMilliseconds} milliseconds";

            return msg;
        }

        public void DrawGizmos(Color32 provColor, Color32 provColor2, int mode){}
    }
}