using System;
using System.Collections.Generic;
using Milhouzer.CameraSystem;
using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.Core.Player
{
    /// <summary>
    /// InputPaylod contains input information : 
    /// - tick          : tick at which the input was received
    /// - inputVector   : value of the input
    /// </summary>
    /// TODO(MINOR):Create generic InputPayload<T> struct to handle different value types
    public struct InputPayload : INetworkSerializable
    {
        public int Tick;
        public Vector3 InputVector;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref InputVector);
        }
    }

    /// <summary>
    /// StatePayload contains player state information : 
    /// - tick      : tick at which the input was received
    /// - position  : state position of the player
    /// - rotation  : state rotation of the player
    /// - velocity  : state velocity of the player (unused)
    /// </summary>
    public struct StatePayload : INetworkSerializable
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
        }
    }


    public class PlayerController : NetworkBehaviour
    {
        /// <summary>
        /// Movement speed of the player. The input is multiplied by this factor.
        /// </summary>
        [SerializeField]
        private float moveSpeed = 0.1f;

        [SerializeField]
        private CameraController cameraController;
        public CameraController CamController => cameraController;

        [SerializeField]
        private AudioListener audioListener;

        // Controller
        private Vector3 _velocity = Vector3.zero;

        // Netcode general
        private NetworkTimer _timer;
        /// <summary>
        /// FPS the time will be running at on the server
        /// </summary>
        private const float k_serverTickRate = 60f;
        /// <summary>
        /// Size of circular buffers
        /// </summary>
        private const int k_bufferSize = 1024;

        // Netcode client specific
        /// <summary>
        /// Transform states at every tick
        /// </summary>
        private CircularBuffer<StatePayload> _clientStateBuffer;
        /// <summary>
        /// Inputs states at every tick
        /// </summary>
        private CircularBuffer<InputPayload> _clientInputBuffer;
        /// <summary>
        /// Last processed state that came from the server
        /// </summary>
        private StatePayload _lastServerState;
        /// <summary>
        /// Last successfully reconciled state
        /// </summary>
        private StatePayload _lastProcessedState;

        // Netcode server specific
        /// <summary>
        /// Simulated states on the server
        /// </summary>
        private CircularBuffer<StatePayload> _serverStateBuffer;
        /// <summary>
        /// Queue inputs as they come in
        /// </summary>
        private Queue<InputPayload> _serverInputQueue;

        [Header("Netcode")]
        [SerializeField] float reconciliationThreshold = 10f;
        /// <summary>
        /// Represents where the server thinks the client should be
        /// </summary>
        [SerializeField] GameObject serverCube;
        /// <summary>
        /// Represents where the client is according to him
        /// </summary>
        [SerializeField] GameObject clientCube;

        public static event Action<PlayerController> OnPlayerReady;

        /// <summary>
        /// Instantiate variables instances
        /// </summary>
        void Awake() {
            _timer = new NetworkTimer(k_serverTickRate);
            _clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            _clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);

            _serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            _serverInputQueue = new Queue<InputPayload>();
        }

        public override void OnNetworkSpawn() {
            audioListener = cameraController.GetComponent<AudioListener>();

            if (!IsOwner) {
                cameraController.Priority = 0;
                audioListener.enabled = false;
                return;
            }
            
            cameraController.LookAt(transform);
            cameraController.Priority = 100;
            audioListener.enabled = true;

            if(IsOwner) {
                Debug.Log("[PlayerController] player object created");
                OnPlayerReady?.Invoke(this);
            }
        }

        private void Update() {
            _timer.Update(Time.deltaTime);
            if(UnityEngine.Input.GetKeyDown(KeyCode.Q)) {
                // transform.position += transform.forward * 20f;
            }
        }

        void FixedUpdate() {

            while (_timer.ShouldTick()) {
                HandleClientTick();
                HandleServerTick();
            }
        }

        /// <summary>
        ///
        /// </summary>
        void HandleServerTick() {
            if (!IsServer) return;

            int bufferIndex = -1;
            while(_serverInputQueue.Count > 0) {
                InputPayload inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.Tick % k_bufferSize;

                StatePayload statePayload = SimulateMovement(inputPayload);
                serverCube.transform.position = statePayload.Position;
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if(bufferIndex == -1) return;
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex));
        }

        /// <summary>
        ///
        /// </summary>
        private void HandleClientTick() {
            if (!IsClient || !IsOwner) return;

            int currentTick = _timer.CurrentTick;
            int bufferIndex = currentTick % k_bufferSize;

            if(GameManager.Instance.Input.Move == Vector3.zero) return;

            InputPayload inputPayload = new InputPayload() {
                Tick = currentTick,
                InputVector = GameManager.Instance.Input.Move
            };

            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            _clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        /// <summary>
        ///
        /// </summary>
        private void HandleServerReconciliation()
        {
            if(!ShouldReconcile()) return;

            int bufferIndex = _lastServerState.Tick % k_bufferSize;
            if(bufferIndex - 1 < 0) return; // Not enought information to reconcile


            StatePayload rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState; // Host RPCs execute immediately, we can use the last server state
            float posError = Vector3.Distance(_clientStateBuffer.Get(bufferIndex).Position, rewindState.Position);

            if(posError > reconciliationThreshold) {
                Debug.Break();
                ReconcileState(rewindState);
            }

            _lastProcessedState = _lastServerState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rewindState"></param>
        private void ReconcileState(StatePayload rewindState) {
            transform.position = rewindState.Position;
            transform.rotation = rewindState.Rotation;
            _velocity = rewindState.Velocity;

            if(!rewindState.Equals(_lastServerState)) return;

            _clientStateBuffer.Add(rewindState, rewindState.Tick);

            // Replay all inputs from the rewind state to the current state
            int tickToReplay = _lastServerState.Tick;

            while(tickToReplay < _timer.CurrentTick) {
                int bufferIndex = tickToReplay % k_bufferSize;
                StatePayload statePayload = ProcessMovement(_clientInputBuffer.Get(bufferIndex));
                _clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private bool ShouldReconcile() {
            bool isNewServerState = !_lastServerState.Equals(default);
            bool isLastStateUndefinedOrDifferent = _lastProcessedState.Equals(default)
                                                    || !_lastProcessedState.Equals(_lastServerState);

            return isNewServerState && isLastStateUndefinedOrDifferent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="statePayload"></param>
        [ClientRpc]
        private void SendToClientRpc(StatePayload statePayload) {
            serverCube.transform.position = statePayload.Position;

            if (!IsOwner) return;
            _lastServerState = statePayload;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        [ServerRpc]
        private void SendToServerRpc(InputPayload inputPayload)
        {
            _serverInputQueue.Enqueue(inputPayload);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        /// <returns></returns>
        private StatePayload SimulateMovement(InputPayload inputPayload) {
            Physics.simulationMode = SimulationMode.Script;

            Move(inputPayload.InputVector);
            Physics.Simulate(Time.fixedDeltaTime);

            Physics.simulationMode = SimulationMode.FixedUpdate;

            return new StatePayload() {
                Tick = inputPayload.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _velocity
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        /// <returns></returns>
        private StatePayload ProcessMovement(InputPayload inputPayload){
            Move(inputPayload.InputVector);

            return new StatePayload() {
                Tick = inputPayload.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _velocity
            };
        }

        /// <summary>
        /// Movement logic.
        /// </summary>
        /// <param name="input"></param>
        private void Move(Vector3 input) {
            transform.position += input * moveSpeed;
        }
    }
}
