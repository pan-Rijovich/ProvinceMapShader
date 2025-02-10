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
        private readonly MapBorderData _data;
        private readonly bool _showExecutionInfo;
        private float _executionTime;

        private IStorageService _storage = new BinaryStorageService();

        public LoadPointsStep(MapBorderData data, bool showExecutionInfo = false)
        {
            _data = data;
            _showExecutionInfo = showExecutionInfo;
        }

        public async Task Execute()
        {
            _stopwatch.Restart();
            
            _storage.Load<BorderSaveData[]>("BorderPoints", 
                obj => _data.BordersSaveData = obj);
            
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