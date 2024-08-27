using System;
using Milhouzer.Input;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.BuildingSystem
{
    /// <summary>
    /// PreviewSettings holds vizualisation and technical settings: preview model material, collision layer, etc.
    /// </summary>
    [Serializable]
    public class PreviewSettings {
        public Material Material;
        public int Layer = 10;
    }
    
    /// <summary>
    /// BuildPayload contains information to pass on the network that will be passed in the build server rpc :
    /// - index of the object to build in the catalog
    /// - position, rotation and scale of the object
    /// </summary>
    public struct BuildPayload : INetworkSerializable
    {
        public int index;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public BuildPayload(int i, Vector3 pos, Quaternion rot, Vector3 s)
        {
            index = i;
            position = pos;
            rotation = rot;
            scale = s;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref index);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref scale);
        }
    }
    

    /// <summary>
    /// BuildManager handles the building system, objects are previewed locally, a ServerRpc is sent to spawn objects on the network.
    /// </summary>
    public class BuildManager : NetworkedSingleton<BuildManager> 
    {
        /// <summary>
        /// Objects that can be built.
        /// </summary>
        [SerializeField] BuildableCatalog catalog;
        /// <summary>
        /// Preview settings.
        /// </summary>
        [SerializeField] PreviewSettings previewSettings;
        /// <summary>
        /// Input used to detect entering/exiting build mode.
        /// </summary>
        IGameInput input;
        /// <summary>
        /// Camera used to raycast for preview placement.
        /// </summary>
        Camera cam;
        /// <summary>
        /// Preview handler used to preview buildings in the world.
        /// </summary>
        BuilderPreview preview;
        /// <summary>
        /// Current selected buildable.
        /// </summary>
        int index = 0;
        /// <summary>
        /// True if is build mode, false otherwise.
        /// </summary>
        bool _isBuilding;

        public delegate void EnterBuildMode();
        public event EnterBuildMode OnEnterBuildMode;
        public delegate void ExitBuildMode();
        public event ExitBuildMode OnExitBuildMode;

        /// <summary>
        /// Init method, register callbacks for client and initialize objects catalog on both client and server.
        /// </summary>
        /// <param name="settings"></param>
        public void Init(BuildManagerSettings settings)
        {
            catalog = settings.Catalog;
            catalog.Init();

            if(IsClient) {
                previewSettings = settings.PreviewSettings;
                input = settings.Input;
                cam = settings.Camera;

                input.OnEnterBuildModeEvent += EnterBuildMode_Internal;
                input.OnExitBuildModeEvent += ExitBuildMode_Internal;
                input.OnBuildEvent += OnBuild;
            }
        }

        /// <summary>
        /// OnDestroy Unity event: unregister input callbacks for client
        /// </summary>
        public override void OnDestroy() {
            base.OnDestroy();

            if(IsClient && input != null)
            {
                input.OnEnterBuildModeEvent -= EnterBuildMode_Internal;
                input.OnExitBuildModeEvent -= ExitBuildMode_Internal;
                input.OnBuildEvent -= OnBuild;
            }
        }
        
        private void Update() {
            HandleClientTick();
        }

        /// <summary>
        /// If in building mode, preview object at the pointed position.
        /// </summary>
        private void HandleClientTick()
        {
            if(IsServer) return;

            if(_isBuilding && cam != null) 
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 150f, ~(1 << 10)))
                {
                    Preview(hit.point);
                }
            }
        }

        /// <summary>
        /// Preview the object at the specified position, rotation and scale
        /// </summary>
        /// <param name="pos"></param>
        private void Preview(Vector3 pos) {
            if(preview == null) {
                Debug.LogWarning("preview is null");
                return;
            }

            preview.Preview(pos, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// Enter build mode input callback
        /// </summary>
        private void EnterBuildMode_Internal() {
            if(_isBuilding) return;

            Debug.Log("[BuildManager] enter building mode");
            preview = new BuilderPreview(catalog[index], previewSettings);
            _isBuilding = true;
            OnEnterBuildMode?.Invoke();
        }

        /// <summary>
        /// Exit build mode input callback
        /// </summary>
        private void ExitBuildMode_Internal() {
            if(!_isBuilding) return;

            Debug.Log("[BuildManager] exit building mode");
            if (preview != null) {
                preview.Cleanup();
                preview = null;
            }
            _isBuilding = false;
            OnExitBuildMode?.Invoke();
        }

        /// <summary>
        /// Sends a server rpc to build the selected object.
        /// </summary>
        private void OnBuild()
        {
            BuildPayload payload = new BuildPayload(
                index,
                preview.Position,
                preview.Rotation,
                Vector3.one

            );
            BuildServerRpc(payload);
        }

        /// <summary>
        /// Spawn the selected object on the network for all clients.
        /// </summary>
        /// <param name="inputPayload"></param>
        [ServerRpc(RequireOwnership = false)]
        void BuildServerRpc(BuildPayload buildPayload)
        {
            if(!IsServer) return;
            
            BuildableElement elem = catalog[buildPayload.index];
            GameObject instantiatedObject = Instantiate(elem.Object.gameObject, buildPayload.position, buildPayload.rotation);
            NetworkObject networkObject = instantiatedObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("The object to be instantiated must have a NetworkObject component attached.");
            }
        }
    }
}