using Fusion;
using PhotonFusionGiris.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Loops
{
    public class PlayerCharacterController : NetworkBehaviour
    {
        [SerializeField] NetworkCharacterControllerPrototype _networkCharacterControllerPrototype;

        public InputReaderLoop Input { get; set; }

        void Awake()
        {
            GetReference();
            Input = new InputReaderLoop();
        }

        void OnValidate()
        {
            GetReference();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out InputDataLoop input))
            {
                _networkCharacterControllerPrototype.Move(input.MoveDirection);
            }
        }

        private void GetReference()
        {
            if (_networkCharacterControllerPrototype == null)
            {
                _networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
            }
        }
    }

    public class InputReaderLoop
    {
        public Vector3 MovePosition { get; private set; }

        public InputReaderLoop()
        {
            GameInputActions input = new GameInputActions();

            input.Player.Move.performed += HandleOnMove;
            input.Player.Move.canceled += HandleOnMove;

            input.Enable();
        }

        void HandleOnMove(InputAction.CallbackContext context)
        {
            Vector2 inputValue = context.ReadValue<Vector2>();
            MovePosition = new Vector3(inputValue.x, 0f, inputValue.y);
        }
    }

    public struct InputDataLoop : INetworkInput
    {
        public Vector3 MoveDirection { get; set; }
    }
}