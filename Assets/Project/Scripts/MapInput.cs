using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapInput : MonoBehaviour
    {
        [SerializeField] Mapshower _mapshower;

        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            var mousePos = Input.mousePosition;
            var ray = _cam.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if(_mapshower.enabled)_mapshower.SelectProvince(hitInfo.textureCoord);
            }
        }
    }
}