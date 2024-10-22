using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer.Input {
    public struct InputEvent {
        public InputAction.CallbackContext Context;
        public bool Cancel;
    }

    public delegate void CancelableInputEvent(ref bool Cancel);

    public interface IGameInput : 
            PlayerInputActions.IPlayerActions,
            PlayerInputActions.IBuiderActions,
            PlayerInputActions.IAlternativesActions,
            PlayerInputActions.IGenericsActions { }

    public interface IGameInputEventSender : IBuildInputEventSender, IUIInputEventSender { }

    public interface IBuildInputEventSender
    {
        public event Action OnEnterBuildModeEvent;
        public event Action OnExitBuildModeInput;
        public event CancelableInputEvent OnBuildInput;
        public event Action OnUpdateBuildInput;
        public event Action OnResetBuildInput;
        public event Action<Vector2> OnRotateBuildInput;
        public event Action<Vector2> OnScaleBuildInput;
        public event Action<int> OnSelectBuildInput;
    }

    public interface IUIInputEventSender 
    {
        public event CancelableInputEvent  OnUseContextActionInput;
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "Input")]
    public class InputReader : ScriptableObject, IGameInput, IGameInputEventSender
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
        private bool Control = false;
        private bool Alt = false;

        PlayerInputActions inputActions;

        private void OnEnable() {
            if(inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
                inputActions.Buider.SetCallbacks(this);
                inputActions.Alternatives.SetCallbacks(this);
                inputActions.Generics.SetCallbacks(this);
            }

            inputActions.Enable();
            inputActions.Buider.Disable();
        }

        #region INPUT MODES

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

        #endregion

        //**************************//
        //  GENERICS INPUT SECTION  //
        //**************************//
        #region GENERIC INPUTS     

        public void OnSelect(InputAction.CallbackContext context)
        {
            InputEvent e = new InputEvent() {
                Context = context,
                Cancel = false
            };

            ProcessEvent(ref e, ProcessSelect_UI);
            ProcessEvent(ref e, ProcessSelect_Build);
        }

        public void OnScroll(InputAction.CallbackContext context)
        {
            InputEvent e = new InputEvent() {
                Context = context,
                Cancel = false
            };

            ProcessEvent(ref e, ProcessScroll_UI);
            ProcessEvent(ref e, ProcessScroll_Build);
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            InputEvent e = new InputEvent() {
                Context = context,
                Cancel = false
            };

            ProcessEvent(ref e, ProcessCancel_UI);
            ProcessEvent(ref e, ProcessCancel_Build);
        }

        #endregion
        
        //*******************************//
        //   ALTERNATIVE INPUT SECTION   //
        //*******************************//
        #region ALTERNATIVE INPUTS 

        public void OnShift(InputAction.CallbackContext context)
        {
            Shift = context.ReadValueAsButton();
            Debug.Log("[InputReader] Shift: " + Shift);
        }

        public void OnControl(InputAction.CallbackContext context)
        {
            Control = context.ReadValueAsButton();
            Debug.Log("[InputReader] Shift: " + Control);
        }

        public void OnAlt(InputAction.CallbackContext context)
        {
            Alt = context.ReadValueAsButton();
            Debug.Log("[InputReader] Alt: " + Alt);
        }

        #endregion

        //**************************//
        //  PLAYER INPUT SECTION   //
        //**************************//

        #region PLAYER INPUTS

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

        #endregion

        //**************************//
        //  BUILDER INPUT SECTION   //
        //**************************//
        #region BUILDER INPUTS        

        public void OnExitBuildMode(InputAction.CallbackContext context)
        {
            Debug.Log("[InputReader] OnExitBuildMode");
            EnablePlayerCallbacks();
            OnExitBuildModeInput?.Invoke();
        }

        #endregion

        #region BUSINESS EVENTS

        public event Action OnEnterBuildModeEvent;
        public event Action OnExitBuildModeInput;
        public event CancelableInputEvent OnBuildInput;

        // TODO(FIX): Delete
        public event Action OnUpdateBuildInput;
        public event Action OnResetBuildInput;
        public event Action<Vector2> OnRotateBuildInput;
        public event Action<Vector2> OnScaleBuildInput;
        public event Action<int> OnSelectBuildInput;
        public event CancelableInputEvent OnUseContextActionInput;

        public delegate bool ProcessInputHandler(ref InputEvent e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private bool ProcessEvent(ref InputEvent e, ProcessInputHandler f)
        {
            if(e.Cancel) return false;
            return f(ref e);
        }
        #endregion

        #region UI EVENTS

        private bool ProcessSelect_UI(ref InputEvent e)
        {
            if (e.Context.canceled) goto canceled;
            else return e.Cancel;
            
            canceled:            
            Debug.Log("[InputReader] OnUseContextActionInput");
            OnUseContextActionInput?.Invoke(ref e.Cancel);
            return e.Cancel;
        }

        private bool ProcessScroll_UI(ref InputEvent e)
        {
            return e.Cancel;
        }

        private bool ProcessCancel_UI(ref InputEvent e)
        {
            return false;
        }

        #endregion

        #region BUILD EVENTS

        private bool ProcessSelect_Build(ref InputEvent e)
        {
            if (e.Context.canceled) goto canceled;
            else return e.Cancel;
            
            canceled:
            if(Alt) {
                Debug.Log("[InputReader] OnBuildInput");
                OnUpdateBuildInput?.Invoke();
                return e.Cancel;
            }

            Debug.Log("[InputReader] OnBuildInput");
            OnBuildInput?.Invoke(ref e.Cancel);
            return e.Cancel;
        }

        private bool ProcessScroll_Build(ref InputEvent e)
        {
            if(e.Context.started) goto started;
            else return e.Cancel;
            
            started:

            // TODO(IMPROVEMENT): Move to UI
            if(!Alt && !Shift){
                Vector2 scroll = e.Context.ReadValue<Vector2>();
                int fscroll = (int)Mathf.Sign(scroll.y);
                Debug.Log(fscroll);
                OnSelectBuildInput?.Invoke(fscroll);
                return e.Cancel;
            }

            if(Alt && !Shift){
                Vector2 scroll = e.Context.ReadValue<Vector2>();
                OnScaleBuildInput?.Invoke(scroll);
                return e.Cancel;
            }

            if(Shift && !Alt){
                Vector2 scroll = e.Context.ReadValue<Vector2>();
                OnRotateBuildInput?.Invoke(scroll);
                return e.Cancel;
            }
            return e.Cancel;
        }

        private bool ProcessCancel_Build(ref InputEvent e)
        {
            if(e.Context.started) goto started;
            else return e.Cancel;
            
            started:
            
            if(e.Context.canceled) {
                Debug.Log("[InputReader] OnReset");
                OnResetBuildInput?.Invoke();
            }

            return e.Cancel;
        }

        #endregion
    }
}