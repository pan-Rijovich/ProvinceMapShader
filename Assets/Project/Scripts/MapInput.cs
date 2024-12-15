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
            CheckButtons();
            var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if(_mapshower.enabled)_mapshower.SelectProvince(hitInfo.textureCoord);
            }
        }

        private void CheckButtons()
        {
            var mouse_weel = Input.GetAxis("Mouse ScrollWheel");
            var scrollOffset = mouse_weel * 0.5f * -_cam.transform.position.z;
            var pos = _cam.transform.position;

            pos += new Vector3(0, 0, scrollOffset);
            pos.z = Mathf.Clamp(pos.z, -900f, -15f);
            _cam.transform.position = pos;


            var moveOffset = -_cam.transform.position.z * 0.005f;
            if (Input.GetKey(KeyCode.D))
            {
                _cam.transform.position += new Vector3(moveOffset, 0, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _cam.transform.position += new Vector3(-moveOffset, 0, 0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                _cam.transform.position += new Vector3(0, moveOffset, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _cam.transform.position += new Vector3(0, -moveOffset, 0);
            }
        }
    }
}