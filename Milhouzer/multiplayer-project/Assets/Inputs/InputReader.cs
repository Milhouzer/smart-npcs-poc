using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer.Input {
    public interface IGameInput : PlayerInputActions.IPlayerActions, PlayerInputActions.IBuiderActions
    {
        public event Action OnEnterBuildModeEvent;
        public event Action OnExitBuildModeEvent;
        public event Action OnBuildEvent;
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IGameInput
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

        public event Action OnEnterBuildModeEvent;
        public event Action OnExitBuildModeEvent;
        public event Action OnBuildEvent;
        
        private void OnEnable() {
            if(inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                inputActions.Buider.SetCallbacks(this);
            }

            inputActions.Enable();
            inputActions.Buider.Disable();
        }

        private void EnablePlayerCallbacks()
        {
            inputActions.Player.Enable();
            inputActions.Buider.Disable();
        }

        private void EnableBuilderCallbacks()
        {
            inputActions.Buider.Enable();
            inputActions.Player.Disable();
        }
        
        //**************************//
        //  PLAYER INPUT SECTION   //
        //**************************//

        public void OnMove(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnEnterBuildMode(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnEnterBuildMode");
            EnableBuilderCallbacks();
            OnEnterBuildModeEvent?.Invoke();
        }

        //**************************//
        //  BUILDER INPUT SECTION   //
        //**************************//
        public void OnBuild(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("[InputReader] OnBuild");
                OnBuildEvent?.Invoke();
            }
        }

        public void OnScale(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnScale");
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnReset");
        }

        public void OnScroll(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnScroll");
        }

        public void OnExitBuildMode(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnExitBuildMode");
            EnablePlayerCallbacks();
            OnExitBuildModeEvent?.Invoke();
        }
    }
}