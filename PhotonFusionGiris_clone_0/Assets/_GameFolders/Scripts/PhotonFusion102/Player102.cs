using Fusion;
using UnityEngine;

namespace Level102
{
    public class Player102 : NetworkBehaviour,ISpawned
    {
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] NetworkCharacterControllerPrototype _characterControllerNetwork;

        public InputReader102 InputReader { get; set; }   

        void Awake()
        {
            GetReference();
            InputReader = new InputReader102();
        }

        void OnValidate()
        {
            GetReference();
        }

        private void GetReference()
        {
            if (_characterControllerNetwork == null)
            {
                _characterControllerNetwork = GetComponentInChildren<NetworkCharacterControllerPrototype>();
            }
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManagerLevel102.Instance.SetLocalPlayer(this);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputDataEntity inputDataEntity))
            {
                if (inputDataEntity.PositionX != 0f || inputDataEntity.PositionZ != 0f)
                {
                    Vector3 direction = new Vector3(inputDataEntity.PositionX, 0f, inputDataEntity.PositionZ);
                    direction = direction * _moveSpeed * Runner.DeltaTime;
                    _characterControllerNetwork.Move(direction);    
                }
            }
        }
    }
}