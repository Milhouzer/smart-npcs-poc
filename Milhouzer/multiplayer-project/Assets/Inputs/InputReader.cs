using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer.Input {
    public interface IGameInput : 
            IBuildInput, 
            PlayerInputActions.IPlayerActions,
            PlayerInputActions.IBuiderActions, 
            PlayerInputActions.IAlternativesActions
    {
        public event Action OnEnterBuildModeEvent;
    }

    public interface IBuildInput
    {
        public event Action OnExitBuildModeEvent;
        public event Action OnBuildEvent;
        public event Action OnResetEvent;
        public event Action<Vector2> OnRotateEvent;
        public event Action<Vector2> OnScaleEvent;
        public event Action<int> OnSelectEvent;
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

        private bool Shift = false;
        private bool Alt = false;

        PlayerInputActions inputActions;

        public event Action OnEnterBuildModeEvent;
        public event Action OnExitBuildModeEvent;
        public event Action OnBuildEvent;
        public event Action OnResetEvent;
        public event Action<Vector2> OnRotateEvent;
        public event Action<Vector2> OnScaleEvent;
        public event Action<int> OnSelectEvent;

        private void OnEnable() {
            if(inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                inputActions.Buider.SetCallbacks(this);
                inputActions.Alternatives.SetCallbacks(this);
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
        
        //*******************************//
        //   ALTERNATIVE INPUT SECTION   //
        //*******************************//
        public void OnShift(InputAction.CallbackContext context)
        {
            Shift = context.ReadValueAsButton();
            Debug.Log("[InputReader] Shift: " + Shift);
        }

        public void OnAlt(InputAction.CallbackContext context)
        {
            Alt = context.ReadValueAsButton();
            Debug.Log("[InputReader] Alt: " + Alt);
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

        public void OnScroll(InputAction.CallbackContext context)
        {
            if(!Alt && !Shift){
                Vector2 scroll = context.ReadValue<Vector2>();
                int fscroll = (int)Mathf.Sign(scroll.y);
                Debug.Log(fscroll);
                OnSelectEvent?.Invoke(fscroll);
            }

            if(Alt && !Shift){
                Vector2 scroll = context.ReadValue<Vector2>();
                OnScaleEvent?.Invoke(scroll);
            }

            if(Shift && !Alt){
                Vector2 scroll = context.ReadValue<Vector2>();
                OnRotateEvent?.Invoke(scroll);
            }
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            if(context.canceled) {
                Debug.Log("[InputReader] OnReset");
                OnResetEvent?.Invoke();
            }
        }

        public void OnExitBuildMode(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnExitBuildMode");
            EnablePlayerCallbacks();
            OnExitBuildModeEvent?.Invoke();
        }
    }
}