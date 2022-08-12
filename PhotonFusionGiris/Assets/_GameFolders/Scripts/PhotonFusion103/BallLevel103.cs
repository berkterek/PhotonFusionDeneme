using Fusion;
using UnityEngine;

namespace Level103
{
    public class BallLevel103 : NetworkBehaviour
    {
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _maxLifeTime = 5f;

        Transform _transform;
        
        [Networked] TickTimer LifeTime { get; set; }

        void Awake()
        {
            _transform = transform;
        }

        void Start()
        {
            LifeTime = TickTimer.CreateFromSeconds(Runner, _maxLifeTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (LifeTime.Expired(Runner))
            {
                Runner.Despawn(this.Object);
                return;
            }
            
            _transform.Translate(Vector3.forward * _moveSpeed * Runner.DeltaTime);
        }
    }
}