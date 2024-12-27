using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapBorderRenderer
{
    public class PointsClearStep : IBorderCreationStep
    {
        private MapBorderData _data;

        public PointsClearStep(MapBorderData data)
        {
            _data = data;
        }

        public async Task Execute()
        {
            
        }
    }
}