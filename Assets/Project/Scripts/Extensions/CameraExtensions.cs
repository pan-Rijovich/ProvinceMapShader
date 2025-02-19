namespace UnityEngine
{
    public static class CameraExtensions
    {
        public static Vector3 GetMouseWorldPositionWithZ(this Camera cam, float z = 0f) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane xyPlane = new Plane(Vector3.forward, Vector3.zero);
            Vector3 mouseWorldPosition = Vector3.zero;

            if (xyPlane.Raycast(ray, out var distance))
            {
                mouseWorldPosition = ray.GetPoint(distance);
            }
            mouseWorldPosition.z = z;

            return mouseWorldPosition;
        }
        
        public static bool GetTouchWorldPositionWithZ(this Camera cam, out Vector3 worldPosition, float z = 0f) 
        {
            worldPosition = Vector3.zero;
            if (Input.touchCount > 0)
            {
                Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                Plane xyPlane = new Plane(Vector3.forward, Vector3.zero);

                if (xyPlane.Raycast(ray, out var distance))
                {
                    worldPosition = ray.GetPoint(distance);
                }
                worldPosition.z = z;

                return true;
            }
            return false;
        }
    }
}