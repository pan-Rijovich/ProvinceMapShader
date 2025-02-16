using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapInput
    {
        private MapShower _mapShower;
        private Camera _cam;

        public MapInput(MapShower mapShower)
        {
            _mapShower = mapShower;
            _cam = Camera.main;
        }

        public void Update()
        {
            var mousePos = Input.mousePosition;
            var ray = _cam.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                _mapShower.SelectProvince(hitInfo.textureCoord);
            }
        }
    }
}