using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer.Input {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        public Vector3 Move
        {
            get
            {
                Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
                return new Vector3(input.x, 0, input.y);
            }
        }
        PlayerInputActions inputActions;

        private void OnEnable() {
            if(inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }

            inputActions.Enable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            // noop
        }
    }
}