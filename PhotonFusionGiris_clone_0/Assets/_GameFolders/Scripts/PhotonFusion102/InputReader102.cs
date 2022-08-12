using Fusion;
using UnityEngine;
using PhotonFusionGiris.Inputs;
using UnityEngine.InputSystem;

namespace Level102
{
    public class InputReader102
    {
        readonly GameInputActions _input;

        public Vector2 Direction { get; private set; }
        
        public InputReader102()
        {
            _input = new GameInputActions();

            _input.Player.Move.performed += HandleOnMove;
            _input.Player.Move.canceled += HandleOnMove;
            
            _input.Enable();
        }

        ~InputReader102()
        {
            _input.Player.Move.performed -= HandleOnMove;
            _input.Player.Move.canceled -= HandleOnMove;
            
            _input.Disable();
        }

        void HandleOnMove(InputAction.CallbackContext context)
        {
            Direction = context.ReadValue<Vector2>();
        }
    }

    public struct InputDataEntity : INetworkInput
    {
        public float PositionX;
        public float PositionZ;
    }
}