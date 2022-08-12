using UnityEngine;

namespace Level103
{
    public class MoveWithTranslate
    {
        readonly Transform _transform;
        readonly float _moveSpeed = 5;
        
        public MoveWithTranslate(Transform transform)
        {
            _transform = transform;
        }
        
        public void FixedTick(Vector2 direction)
        {
            Vector3 newDirection = new Vector3(direction.x, 0f, direction.y);
            newDirection *= _moveSpeed;
            _transform.Translate(newDirection);
        }
    }
}