using Fusion;
using PhotonFusionGiris.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Level105
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkCharacterControllerPrototype))]
    public class PlayerLevel105 : NetworkBehaviour, ISpawned
    {
        [SerializeField] float _fireRate = 0.5f;
        [SerializeField] NetworkCharacterControllerPrototype _networkCharacterController;
        [SerializeField] PhysicBall105 _physicBall105Prefab;
        [SerializeField] MeshRenderer _childMeshRenderer;
        [SerializeField] Color _beforeFireColor;
        [SerializeField] Color _afterFireColor;

        Transform _transform;

        public InputReaderLevel105 InputReader { get; set; }
        [Networked] TickTimer FireRateTimer { get; set; }
        
        [Networked(OnChanged = nameof(HandleOnSpawnedValueChanged))] 
        NetworkBool IsBallSpawned { get; set; }

        void Awake()
        {
            GetReference();
            InputReader = new InputReaderLevel105();
            _transform = transform;
        }

        void OnValidate()
        {
            GetReference();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputDataEntity105 inputData))
            {
                Vector2 oldMoveDirection = inputData.MoveDirection;
                Vector3 moveDirection = new Vector3(oldMoveDirection.x, 0f, oldMoveDirection.y);
                _networkCharacterController.Move(moveDirection);

                if (inputData.IsFire)
                {
                    if (FireRateTimer.ExpiredOrNotRunning(Runner))
                    {
                        Runner.Spawn(_physicBall105Prefab, _transform.forward + _transform.position,
                            _transform.rotation, Object.InputAuthority,
                            (networkRunner, networkObject) =>
                                networkObject.GetComponent<PhysicBall105>().InitForNetwork());
                        FireRateTimer = TickTimer.CreateFromSeconds(Runner, _fireRate);
                        IsBallSpawned = true;
                    }
                    else
                    {
                        IsBallSpawned = false;
                    }
                }
            }
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManagerLevel105.Instance.NetworkRunnerHelper.SetLocalPlayer(this);
            }
        }

        private static void HandleOnSpawnedValueChanged(Changed<PlayerLevel105> player)
        {
            var behaviour = player.Behaviour;
            behaviour._childMeshRenderer.material.color = behaviour._afterFireColor;
        }

        public override void Render()
        {
            var material = _childMeshRenderer.material;
            material.color = Color.Lerp(material.color, _beforeFireColor, Time.deltaTime);
        }

        private void GetReference()
        {
            if (_networkCharacterController == null)
            {
                _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
            }

            if (_childMeshRenderer == null)
            {
                _childMeshRenderer = GetComponentInChildren<MeshRenderer>();
            }
        }
    }

    public struct InputDataEntity105 : INetworkInput
    {
        public Vector2 MoveDirection { get; set; }
        public bool IsFire { get; set; }
    }

    public class InputReaderLevel105
    {
        public Vector2 MoveDirection { get; private set; }
        public bool Fire { get; private set; }

        public InputReaderLevel105()
        {
            GameInputActions input = new GameInputActions();

            input.Player.Move.performed += HandleOnMove;
            input.Player.Move.canceled += HandleOnMove;

            input.Player.Jump.performed += HandleOnFire;
            input.Player.Jump.canceled += HandleOnFire;

            input.Enable();
        }

        void HandleOnFire(InputAction.CallbackContext context)
        {
            Fire = context.ReadValueAsButton();
        }

        void HandleOnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }
    }
}