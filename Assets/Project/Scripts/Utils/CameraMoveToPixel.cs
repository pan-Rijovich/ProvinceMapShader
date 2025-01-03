using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class CameraMoveToPixel : MonoBehaviour
    {
        [SerializeField] Vector2Int _targetPixelCoordinates = new(512, 512);
        [SerializeField] Vector2Int _targetHalfPixelCoordinates = new(1024, 1024);
        [SerializeField] int _targetIndex = 512;
        
        private Camera _cam;
        private Vector2Int _mapSize = new(5632, 2048);
        private Vector2 _startPoint;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _startPoint = new(_mapSize.x / -2f, _mapSize.y / -2f);
        }

        [ContextMenu("GoToTargetPixelCoordinates")]
        private void GoToTargetPixelCoordinates()
        {
            _cam ??= Camera.main;
            var z = _cam.transform.position.z;
            var target = _startPoint + new Vector2(_targetPixelCoordinates.x, _targetPixelCoordinates.y);
            _cam.transform.position = new(target.x, target.y, z);
        }
        
        [ContextMenu("GoToTargetHalfPixelCoordinates")]
        private void GoToTargetHalfPixelCoordinates()
        {
            _cam ??= Camera.main;
            var z = _cam.transform.position.z;
            var target = _startPoint + new Vector2(_targetHalfPixelCoordinates.x / 2f, _targetHalfPixelCoordinates.y / 2f);
            _cam.transform.position = new(target.x, target.y, z);
        }
        [ContextMenu("GoToTargetIndex")]
        private void GoToTargetIndex()
        {
            _cam ??= Camera.main;
            var z = _cam.transform.position.z;
            var coordinates = new Vector2(_targetIndex % _mapSize.x, _targetIndex / _mapSize.x);
            var target = _startPoint + coordinates;
            _cam.transform.position = new(target.x, target.y, z);
            Debug.Log(coordinates);
        }
        
    }
}