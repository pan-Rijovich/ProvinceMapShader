using System.Threading.Tasks;

namespace MapBorderRenderer
{
    public class WriteBordersInPovinceStep : IBorderCreationStep
    {
        private MapBorderData _data;

        public WriteBordersInPovinceStep(MapBorderData data)
        {
            _data = data;
        }

        public async Task Execute()
        {
            foreach (var borderID in _data.Borders.Keys)
            {
                uint province1ID = (uint)borderID;
                uint province2ID = (uint)(borderID >> 32);

                _data.AddToListInDictionary(province1ID, borderID, _data.ProvincesBorders);
                _data.AddToListInDictionary(province2ID, borderID, _data.ProvincesBorders);
            }
        }
    }
}