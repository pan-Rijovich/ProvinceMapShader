using R3;
using UnityEngine;

namespace Assets.Scripts.CameraController
{
    public class CameraController : MonoBehaviour
    {
        public readonly ReactiveProperty<float> Zoom = new(0.5f);
        
        private Camera _cam;
        private float _maxZoomOffset = -900f;
        private float _minZoomOffset = -5f;
        private Vector3 _maxZoomRotation = new Vector3(0f, 0f, 0f);
        private Vector3 _minZoomRotation = new Vector3(-30f, 0f, 0f);
        private float _wheelNormalizedStep = -0.125f;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void Update()
        {
            var mouse_weel = Input.GetAxis("Mouse ScrollWheel") * _wheelNormalizedStep;
            var zoom = Mathf.Clamp(Zoom.Value + mouse_weel, 0f, 1f);
            
            var positionT = zoom * zoom;
            var pos = transform.position;
            pos.z = Mathf.Lerp(_minZoomOffset, _maxZoomOffset, positionT);
            transform.position = pos;
            
            var rotationT = Mathf.Lerp(0f, 1f, zoom * 4f);
            rotationT *= rotationT;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(_minZoomRotation), Quaternion.Euler(_maxZoomRotation), rotationT);

            Move();
            Zoom.Value = zoom;
        }

        private void Move()
        {
            var moveOffset = -transform.position.z * 0.005f;
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