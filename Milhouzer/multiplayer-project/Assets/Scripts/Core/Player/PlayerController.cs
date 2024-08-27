using System;
using System.Collections.Generic;
using Milhouzer.BuildingSystem;
using Milhouzer.CameraSystem;
using Milhouzer.Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer
{
    /// <summary>
    /// InputPaylod contains input information : 
    /// - tick          : tick at which the input was received
    /// - inputVector   : value of the input
    /// </summary>
    /// <TODO>
    /// Create generic InputPayload<T> struct to handle different value types
    /// </TODO>
    public struct InputPayload : INetworkSerializable
    {
        public int tick;
        public Vector3 inputVector;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
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
        public int tick;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref velocity);
        }
    }


    public class PlayerController : NetworkBehaviour
    {
        /// <summary>
        /// Movement speed of the player. The input is multiplied by this factor.
        /// </summary>
        [SerializeField]
        float moveSpeed = 0.1f;

        [SerializeField]
        CameraController cameraController;
        public CameraController CamController => cameraController;

        [SerializeField]
        AudioListener audioListener;

        // Controller
        Vector3 velocity = Vector3.zero;

        // Netcode general
        NetworkTimer timer;
        /// <summary>
        /// FPS the time will be running at on the server
        /// </summary>
        const float k_serverTickRate = 60f;
        /// <summary>
        /// Size of circular buffers
        /// </summary>
        const int k_bufferSize = 1024;

        // Netcode client specific
        /// <summary>
        /// Transform states at every tick
        /// </summary>
        CircularBuffer<StatePayload> clientStateBuffer;
        /// <summary>
        /// Inputs states at every tick
        /// </summary>
        CircularBuffer<InputPayload> clientInputBuffer;
        /// <summary>
        /// Last processed state that came from the server
        /// </summary>
        StatePayload lastServerState;
        /// <summary>
        /// Last successfully reconciled state
        /// </summary>
        StatePayload lastProcessedState;

        // Netcode server specific
        /// <summary>
        /// Simulated states on the server
        /// </summary>
        CircularBuffer<StatePayload> serverStateBuffer;
        /// <summary>
        /// Queue inputs as they come in
        /// </summary>
        Queue<InputPayload> serverInputQueue;

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

        public static event Action<PlayerController> OnPlayerCreatedCallback;

        /// <summary>
        /// Instantiate variables instances
        /// </summary>
        void Awake() {
            timer = new NetworkTimer(k_serverTickRate);
            clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);

            serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            serverInputQueue = new Queue<InputPayload>();
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
                OnPlayerCreatedCallback?.Invoke(this);
            }
        }

        void Update() {
            timer.Update(Time.deltaTime);
            if(UnityEngine.Input.GetKeyDown(KeyCode.Q)) {
                // transform.position += transform.forward * 20f;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0)) // 0 is the left mouse button
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            Ray ray = cameraController.ScreenToPointRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    GameManager.Instance.SelectServerRpc(hit.transform.gameObject, NetworkManager.Singleton.LocalClientId);
                }
            }
        }

        void FixedUpdate() {

            while (timer.ShouldTick()) {
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
            while(serverInputQueue.Count > 0) {
                InputPayload inputPayload = serverInputQueue.Dequeue();
                bufferIndex = inputPayload.tick % k_bufferSize;

                StatePayload statePayload = SimulateMovement(inputPayload);
                serverCube.transform.position = statePayload.position;
                serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if(bufferIndex == -1) return;
            SendToClientRpc(serverStateBuffer.Get(bufferIndex));
        }

        /// <summary>
        ///
        /// </summary>
        private void HandleClientTick() {
            if (!IsClient || !IsOwner) return;

            int currentTick = timer.CurrentTick;
            int bufferIndex = currentTick % k_bufferSize;

            if(GameManager.Instance.Input.Move == Vector3.zero) return;

            InputPayload inputPayload = new InputPayload() {
                tick = currentTick,
                inputVector = GameManager.Instance.Input.Move
            };

            clientInputBuffer.Add(inputPayload, bufferIndex);
            SendToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        /// <summary>
        ///
        /// </summary>
        private void HandleServerReconciliation()
        {
            if(!ShouldReconcile()) return;

            int bufferIndex = lastServerState.tick % k_bufferSize;
            if(bufferIndex - 1 < 0) return; // Not enought information to reconcile

            float posError;
            StatePayload rewindState;


            rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState; // Host RPCs execute immediately, we can use the last server state
            posError = Vector3.Distance(clientStateBuffer.Get(bufferIndex).position, rewindState.position);

            if(posError > reconciliationThreshold) {
                Debug.Break();
                ReconcileState(rewindState);
            }

            lastProcessedState = lastServerState;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rewindState"></param>
        void ReconcileState(StatePayload rewindState) {
            transform.position = rewindState.position;
            transform.rotation = rewindState.rotation;
            velocity = rewindState.velocity;

            if(!rewindState.Equals(lastServerState)) return;

            clientStateBuffer.Add(rewindState, rewindState.tick);

            // Replay all inputs from the rewind state to the current state
            int tickToReplay = lastServerState.tick;

            while(tickToReplay < timer.CurrentTick) {
                int bufferIndex = tickToReplay % k_bufferSize;
                StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
                clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        bool ShouldReconcile() {
            bool isNewServerState = !lastServerState.Equals(default);
            bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default)
                                                    || !lastProcessedState.Equals(lastServerState);

            return isNewServerState && isLastStateUndefinedOrDifferent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="statePayload"></param>
        [ClientRpc]
        void SendToClientRpc(StatePayload statePayload) {
            serverCube.transform.position = statePayload.position;

            if (!IsOwner) return;
            lastServerState = statePayload;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        [ServerRpc]
        void SendToServerRpc(InputPayload inputPayload)
        {
            serverInputQueue.Enqueue(inputPayload);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        /// <returns></returns>
        StatePayload SimulateMovement(InputPayload inputPayload) {
            Physics.simulationMode = SimulationMode.Script;

            Move(inputPayload.inputVector);
            Physics.Simulate(Time.fixedDeltaTime);

            Physics.simulationMode = SimulationMode.FixedUpdate;

            return new StatePayload() {
                tick = inputPayload.tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = velocity
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputPayload"></param>
        /// <returns></returns>
        StatePayload ProcessMovement(InputPayload inputPayload){
            Move(inputPayload.inputVector);

            return new StatePayload() {
                tick = inputPayload.tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = velocity
            };
        }

        /// <summary>
        /// Movement logic.
        /// </summary>
        /// <param name="input"></param>
        void Move(Vector3 input) {
            transform.position += input * moveSpeed;
        }
    }
}
