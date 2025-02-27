using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Configs;
using Services.Storage;

namespace MapBorderRenderer
{
    public class MapBordersGenerator
    {
        private MapConfig _config;
        private IStorageService _storage = new BinaryStorageService();
        private List<IBorderCreationStep> _steps = new(10);
        private MapBorderGenData _mapData;
        private bool _showExecutionInfo = true;

        public bool IsGenerated { get; private set; }
        
        public MapBordersGenerator(MapConfig config)
        {
            _config = config;
        }

        public async Task Generate()
        {
            if (IsGenerated) return;
            
            _mapData = new(_config);
            
            
            if (_storage.HasFileByKey("BorderPoints"))
            {
                _steps = new()
                {
                    new LoadPointsStep(_mapData, _showExecutionInfo),
                    new DrawStep(_mapData, _config.BorderPrefab, _showExecutionInfo),
                };
            }
            else
            {
                _steps = new()
                {
                    new BorderPixelsCollectionStep(_mapData, _showExecutionInfo),
                    new PointsCreationStep(_mapData, _showExecutionInfo),
                    new SortPointsStep(_mapData, _showExecutionInfo),
                    new FilterStraightPointsStep(_mapData, _showExecutionInfo),
                    new SavePointsStep(_mapData, _showExecutionInfo),
                    new DrawStep(_mapData, _config.BorderPrefab, _showExecutionInfo),
                };
            }
                
            foreach(var step in _steps)
            {
                await step.Execute();
            }
            
            

            _steps = null;
            _mapData = null;
            IsGenerated = true;
        }
    }
}