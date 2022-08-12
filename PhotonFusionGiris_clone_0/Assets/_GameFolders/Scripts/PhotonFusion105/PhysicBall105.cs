using Fusion;
using UnityEngine;

namespace Level105
{
    public class PhysicBall105 : NetworkBehaviour
    {
        [SerializeField] float _forceSpeed = 5000f; 
        [SerializeField] NetworkRigidbody _networkRigidbody;

        [Networked] TickTimer LifeTime { get; set; }

        void Awake()
        {
            GetReference();
        }

        void OnValidate()
        {
            GetReference();
        }

        void Start()
        {
            _networkRigidbody.Rigidbody.velocity += transform.forward * _forceSpeed * Runner.DeltaTime;
        }

        private void GetReference()
        {
            if (_networkRigidbody == null)
            {
                _networkRigidbody = GetComponent<NetworkRigidbody>();
            }
        }

        public void InitForNetwork()
        {
            LifeTime = TickTimer.CreateFromSeconds(Runner,3f);
        }

        public override void FixedUpdateNetwork()
        {
            if (LifeTime.Expired(Runner))
            {
                Runner.Despawn(this.Object);
            }
        }
    }    
}