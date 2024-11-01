using System;
using System.Collections.Generic;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;
using Milhouzer.Input;
using Milhouzer.Core.BuildSystem;
using Milhouzer.Core.Player;
using Milhouzer.UI;
using Utils;

namespace Milhouzer.Core
{
    public enum GameState {
        Idle,
        Build
    }

    public class GameManager : NetworkedSingleton<GameManager>, IManager<GameManagerSettings>
    {
        [Header("Controller")]
        [SerializeField]
        InputReader input;
        public InputReader Input => input;

        [SerializeField]
        GameManagerSettings Settings;

        [SerializeField] 
        
        private List<Color> playerColors;

        private NetworkList<PlayerData> _networkPlayersData;

        /// <summary>
        /// Current game state. Action should be executed when the player is in the appropriate mode. (ex: building objects only in Build) 
        /// </summary>
        /// <value></value>
        private GameState GameState { get; set; }

        /// <summary>
        /// Main camera component, different on each client.
        /// </summary>
        private Camera _playerCamera;
        
        /// <summary>
        /// Current pointed object by the camera (should ignore UI)
        /// </summary>
        public static GameObject CurrentPointedObject;

        #region UNITY EVENTS
        
        private void Awake() {
            _networkPlayersData = new NetworkList<PlayerData>(new List<PlayerData>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            PlayerController.OnPlayerReady += OnPlayerReadyEventHandler;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // InitClientManager is called when the player is ready
            if(IsServer) 
            {
                InitServerManager(Settings);
            }
        }

        private void Update() {
            if(IsClient) HandleClientTick();
        }
        
        /// <summary>
        /// If in building mode, preview object at the pointed position.
        /// </summary>
        private void HandleClientTick()
        {
            if(IsServer) return;

            if (this.GameState != GameState.Build || !_playerCamera) return;
            if (!Physics.Raycast(_playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hit,
                    150f, ~(1 << 10))) return;
            CurrentPointedObject = hit.transform.gameObject;
            BuildManager.Instance.Preview(hit.point);
        }

        #endregion

        /// <summary>
        /// Player connected callback, init <see cref="BuildManager"/> with data injection
        /// </summary>
        /// <param name="controller"></param>
        private void OnPlayerReadyEventHandler(PlayerController controller)
        {
            if (!IsClient) return;
            
            Debug.Log($"[GameManager] player object created callback on server {controller.CamController.Camera}");
            _playerCamera = controller.CamController.Camera;
            InitClientManager(Settings);
        }

        #region INIT MANAGERS
        
        public void InitClientManager(GameManagerSettings managerSettings)
        {
            Debug.Log("[GameManager] Init as client");
            InitClientUIManager(Settings.UIManagerSettings);
            InitClientBuildManager(Settings.BuildManagerSettings);
        }

        public void InitServerManager(GameManagerSettings managerSettings)
        {
            Debug.Log("[GameManager] Init as server");
            InitServerBuildManager(Settings.BuildManagerSettings);
        }
        
        /// <summary>
        /// Init server <see cref="BuildManager"/>, only the catalog is necessary for the server to instantiate objects.
        /// </summary>
        /// <param name="settings"></param>
        private void InitServerBuildManager(BuildManagerSettings settings) {
            settings = Instantiate(settings);
            BuildManager.Instance.InitServerManager(settings);
        }

        /// <summary>
        /// Init client <see cref="BuildManager"/>, register callbacks to control the build system
        /// <see cref="BuildManager.InitClientManager"/>
        /// </summary>
        /// <param name="settings"></param>
        private void InitClientBuildManager(BuildManagerSettings settings) {
            settings = Instantiate(settings);
            BuildManager.Instance.InitClientManager(settings);

            // Register callbacks
            input.OnEnterBuildModeEvent += OnEnterBuildModeInputHandler;
            input.OnExitBuildModeInput += OnExitBuildModeInputHandler;
            input.OnBuildInput += OnBuildInputHandler;
            input.OnRotateBuildInput += BuildManager.Instance.RequestPreviewRotation;
            input.OnScaleBuildInput += BuildManager.Instance.RequestPreviewScale;
            input.OnResetBuildInput += BuildManager.Instance.RequestReset;
            input.OnSelectBuildInput += BuildManager.Instance.RequestSelect;
            input.OnUseContextActionInput += OnUseContextActionInputHandler;
        }

        /// <summary>
        /// Initialize client <see cref="UIManager"/>, see: <see cref="UIManager.InitClientManager"/>
        /// </summary>
        /// <param name="settings"></param>
        private void InitClientUIManager(UIManagerSettings settings)
        {
            settings = Instantiate(settings);
            settings.Camera = _playerCamera;
            UIManager.Instance.InitClientManager(settings);
        }

        #endregion
        
        #region INPUT HANDLERS

        private void OnEnterBuildModeInputHandler()
        {
            if(this.GameState == GameState.Build) return;

            this.GameState = GameState.Build;
            BuildManager.Instance.RequestEnterBuildMode();
        }
        
        private void OnExitBuildModeInputHandler()
        {
            if(this.GameState != GameState.Build) return;

            this.GameState = GameState.Idle;
            BuildManager.Instance.RequestExitBuildMode();
        }

        private void OnUseContextActionInputHandler(ref bool Cancel)
        {
            Cancel = UIManager.Instance.ExecuteContextAction();
            Debug.Log($"[GameManager] UseContextAction event has been processed, canceled: {Cancel}");
        }

        private void OnBuildInputHandler(ref bool Cancel)
        {
            if(this.GameState != GameState.Build) return;

            BuildManager.Instance.RequestBuild();
            Cancel = true;
            Debug.Log($"[GameManager] RequestBuild event has been processed, canceled: {Cancel}");
        }

        #endregion
    }
}
