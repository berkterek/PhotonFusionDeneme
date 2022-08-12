using UnityEngine;

namespace Level103
{
    public class RotationWithMousePosition
    {
        readonly Transform _transform;
        readonly Camera _camera;

        Vector3 _direction;
        
        public RotationWithMousePosition(Transform transform)
        {
            _transform = transform;
            _camera = Camera.main;
        }

        public void FixedTick(Vector2 rotation)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(rotation), out RaycastHit raycastHit,Mathf.Infinity))
            {
                _direction = raycastHit.point - _transform.position;
                _direction.Normalize();
                _direction = new Vector3(_direction.x, 0f, _direction.z);
                _transform.forward = _direction;
                // _transform.LookAt(raycastHit.point);
                // _transform.eulerAngles = new Vector3(0f, _transform.eulerAngles.y, 0f);
            }
        }
    }
}