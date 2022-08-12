using PhotonFusionGiris.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Level103
{
    public class InputReader103
    {
        readonly GameInputActions _input;

        public Vector2 MoveDirection { get; private set; }
        public bool Jump { get; private set; }
        public Vector2 MousePosition { get; private set; }
        
        public InputReader103()
        {
            _input = new GameInputActions();

            _input.Player.Move.performed += HandleOnMove;
            _input.Player.Move.canceled += HandleOnMove;

            _input.Player.Jump.performed += HandleOnJump;
            _input.Player.Jump.canceled += HandleOnJump;

            _input.Player.MousePosition.performed += HandleOnMousePosition;
            
            _input.Enable();
        }

        ~InputReader103()
        {
            _input.Player.Move.performed -= HandleOnMove;
            _input.Player.Move.canceled -= HandleOnMove;
            
            _input.Player.Jump.performed -= HandleOnJump;
            _input.Player.Jump.canceled -= HandleOnJump;
            
            _input.Disable();
        }
        
        void HandleOnJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }

        void HandleOnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }
        
        void HandleOnMousePosition(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }
    }
}