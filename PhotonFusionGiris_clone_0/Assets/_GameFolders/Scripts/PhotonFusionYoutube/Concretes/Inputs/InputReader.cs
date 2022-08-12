using PhotonFusionGiris.Inputs;
using PhotonFusionYoutube.Abstracts.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PhotonFusionYoutube.Inputs
{
    public class InputReader : IInputReader
    {
        public Vector3 DirectionInput { get; private set; }
        public Vector2 RotationInput { get; private set; }
        public bool IsJumpInput { get; private set; }

        public InputReader()
        {
            GameInputActions input = new GameInputActions();

            input.Player.Move.performed += HandleOnMove;
            input.Player.Move.canceled += HandleOnMove;

            input.Player.Look.performed += HandleOnRotation;
            input.Player.Look.canceled += HandleOnRotation;

            input.Player.Jump.performed += HandleOnJump;
            input.Player.Jump.canceled += HandleOnJump;

            input.Enable();
        }

        void HandleOnJump(InputAction.CallbackContext context)
        {
            IsJumpInput = context.ReadValueAsButton();
        }

        void HandleOnMove(InputAction.CallbackContext context)
        {
            Vector2 inputValue = context.ReadValue<Vector2>();
            DirectionInput = new Vector3(inputValue.x, 0f, inputValue.y);
        }
        
        void HandleOnRotation(InputAction.CallbackContext context)
        {
            RotationInput = context.ReadValue<Vector2>();
        }
    }    
}