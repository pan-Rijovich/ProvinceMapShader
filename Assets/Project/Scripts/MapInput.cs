using System.Collections;
using Project.Scripts.Configs;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapInput
    {
        private MapShower _mapShower;
        private Camera _cam;
        private MapConfig _config;

        public MapInput(MapShower mapShower, MapConfig config)
        {
            _mapShower = mapShower;
            _config = config;
            _cam = Camera.main;
        }

        public void Update()
        {
            var mapStart = _config.MapStartPoint;
            var worldPos = _cam.GetMouseWorldPositionWithZ();
            var mapUVX = worldPos.x.Normalize(mapStart.x, mapStart.x + _config.MapSizeInWorld.x);
            var mapUVY = worldPos.y.Normalize(mapStart.y, mapStart.y + _config.MapSizeInWorld.y);
            _mapShower.SelectProvince(new Vector2(mapUVX.Clamp01(), mapUVY.Clamp01()));
        }
    }
}