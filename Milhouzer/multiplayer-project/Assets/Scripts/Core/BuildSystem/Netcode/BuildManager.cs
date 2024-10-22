using System;
using System.Collections.Generic;
using System.Linq;
using Milhouzer.Core.BuildSystem.StatesManagement;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem
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
    /// BuildManager handles the building system, objects are previewed locally, a ServerRpc is sent to spawn objects on the network.
    /// </summary>
    public unsafe class BuildManager : NetworkedSingleton<BuildManager> 
    {
        #region Attributes

        /// <summary>
        /// Objects that can be built.
        /// </summary>
        [SerializeField] BuildableCatalog catalog;

        /// <summary>
        /// Catalog accessor
        /// </summary>
        public BuildableCatalog Catalog => catalog;

        /// <summary>
        /// Preview settings.
        /// </summary>
        [SerializeField] PreviewSettings previewSettings;

        /// <summary>
        /// Camera used to raycast for preview placement.
        /// </summary>
        Camera cam;

        /// <summary>
        /// Preview handler used to preview buildings in the world.
        /// </summary>
        BuilderPreview preview;
        
        /// <summary>
        /// Visuals representations of server states kept by the client only.
        /// </summary>
        // List<BuildEntry> Visuals;

        /// <summary>
        /// Current selected buildable.
        /// </summary>
        int selectedBuildable = 0;

        /// <summary>
        /// <see cref="selectedBuildable"/> accessor.
        /// </summary>
        public int Selected =>  selectedBuildable % catalog.Count();

        /// <summary>
        /// Marker to indicate the selection index changed
        /// </summary>
        Timer UpdatePreviewTimer;

        /// <summary>
        /// Client-side saved states to represent 3d objects.
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IStateObject> States = new();

        #endregion
        #region Client events
            
        public delegate void ManagerInstantiated();
        public static event ManagerInstantiated OnManagerInstantiated;
        public delegate void EnterBuildMode();
        public event EnterBuildMode OnEnterBuildMode;
        public delegate void ExitBuildMode();
        public event ExitBuildMode OnExitBuildMode;
        public delegate void BuildSelectionChange(int index);
        public event BuildSelectionChange OnBuildSelectionChange;

        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            OnManagerInstantiated?.Invoke();
            if (IsServer)
            {
                // Load save, do server logic.
            }else{
                // Visuals = new List<BuildEntry>();
            }
        }

        /// <summary>
        /// Init method, register callbacks for client and initialize objects catalog on both client and server.
        /// </summary>
        /// <param name="settings"></param>
        public void Init(BuildManagerSettings settings)
        {
            catalog = settings.Catalog;

            if(IsClient) {
                previewSettings = settings.PreviewSettings;
                UpdatePreviewTimer = new Timer(0.3f, UpdatePreviewObject);
                SyncVisuals();
                StatesManager.Instance.ReplicatedStates.OnListChanged += OnClientStatesChanged;
            }
        }

        #region Visuals management


        /// <summary>
        /// Method called when the client is initialized. Synchronizes all visuals based on synced states
        /// </summary>
        private void SyncVisuals()
        {
            if(IsServer) return;

            int _index = 0;
            foreach (ReplicatedStateInformation rs in StatesManager.Instance.ReplicatedStates)
            {
                BuildVisual(rs.State, _index, rs.Transform);
            }
        }


        /// <summary>
        /// Callback called when the server states have changed
        /// </summary>
        /// <param name="changeEvent"></param>
        private void OnClientStatesChanged(NetworkListEvent<ReplicatedStateInformation> e)
        {
            
            Debug.Log($"OnClientStateChanged");
            switch(e.Type){
                case NetworkListEvent<ReplicatedStateInformation>.EventType.Add:
                    CreateVisual(e.Index, e.Value);
                    break;
                case NetworkListEvent<ReplicatedStateInformation>.EventType.Value:
                    UpdateVisual(e.Index, e.Value);
                    break;
                case NetworkListEvent<ReplicatedStateInformation>.EventType.RemoveAt:
                    DeleteVisual(e.Index);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void CreateVisual(int index, ReplicatedStateInformation value)
        {
            if(IsServer) return;

            BuildVisual(value.State, index, value.Transform);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void UpdateVisual(int index, ReplicatedStateInformation value)
        {
            if(IsServer) return;

            BuildVisual(value.State, index, value.Transform);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void DeleteVisual(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build a visual c at index i.
        /// If a visual at index i already exists, replace and destroy it
        /// </summary>
        /// <param name="c"></param>
        /// <param name="index"></param>
        /// <param name="tr"></param>
        private void BuildVisual(char c, int index, TransformPayload tr)
        {
            string UID = StatesManager.Instance.GetUID(c);

            if(!catalog.Get(x => x.UID == UID, out BuildableElement elem)) {
                Debug.Log($"{UID}:{c} does not exist in catalog");
                return;
            }

            Debug.Log($"BuildVisual {c}:{UID} at index {index}, transform {tr}");

            GameObject visual = elem.Object.Build(tr.position, tr.rotation, tr.scale);
            if(!visual.TryGetComponent<IStateObject>(out var holder)) {
                Debug.LogError($"State holder missing on {visual}, misconfiguration");
                return;
            }

            holder.UID = UID;
            if(States.TryGetValue(index, out IStateObject existing))
            {
                existing.Destroy();
                States[index] = holder;
                return;
            }

            States.Add(index, holder);
        }

        int GetKeyByValue(IStateObject value)
        {
            foreach (var kvp in States)
            {
                if (kvp.Value.Equals(value))
                {
                    return kvp.Key;
                }
            }
            
            // Handle the case where the value is not found
            return -1;
        }

        #region Preview controls

        /// <summary>
        /// Enter build mode input callback
        /// </summary>
        public void RequestEnterBuildMode() {

            Debug.Log("[BuildManager] enter building mode");
            preview = new BuilderPreview(catalog[selectedBuildable], previewSettings);

            // TODO(OPEN): Move to UI component ??
            UpdatePreviewTimer.Restart();
            OnEnterBuildMode?.Invoke();
        }

        /// <summary>
        /// Exit build mode input callback
        /// </summary>
        public void RequestExitBuildMode() {

            Debug.Log("[BuildManager] exit building mode");
            if (preview != null) {
                preview.Cleanup();
                preview = null;
            }

            // TODO(OPEN): Move to UI component
            UpdatePreviewTimer.Stop();
            OnExitBuildMode?.Invoke();
        }

        /// <summary>
        /// Request selection change on the catalog
        /// </summary>
        /// <param name="amount"></param>
        internal void RequestSelect(int amount)
        {
            selectedBuildable = (selectedBuildable + amount % catalog.Count() + catalog.Count()) % catalog.Count();
            NotifyIndexChanged_Internal();
        }

        /// <summary>
        /// Notify that the selection has changed, reset the timer to update the preview.
        /// </summary>
        private void NotifyIndexChanged_Internal()
        {
            UpdatePreviewTimer.Restart();
            OnBuildSelectionChange?.Invoke(Selected);
        }

        /// <summary>
        /// Preview the object at the specified position, rotation and scale
        /// </summary>
        /// <param name="pos"></param>
        public void Preview(Vector3 pos) {
            if(preview == null) {
                Debug.LogWarning("preview is null");
                return;
            }

            if(UpdatePreviewTimer.IsRunning){
                UpdatePreviewTimer.Update(Time.deltaTime);
            } 
            preview.Preview(pos, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePreviewObject() {
            Debug.Log("[BuildManager] update preview for index " + selectedBuildable);
            preview.Cleanup();
            preview = new BuilderPreview(catalog[Mathf.Abs(selectedBuildable)], previewSettings);
            UpdatePreviewTimer.Stop();
        }

        /// <summary>
        /// Input handle to rotate 
        /// </summary>
        public void RequestPreviewRotation(Vector2 amount) {
            preview.Rotate(amount.y);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestPreviewScale(Vector2 amount) {
            preview.Scale(amount.y);
        }

        /// <summary>
        /// Sends a server rpc to build the selected object.
        /// </summary>
        public void RequestBuild()
        {
            BuildPayload payload = new BuildPayload(
                selectedBuildable,
                preview.Position,
                preview.Rotation,
                preview.LocalScale

            );
            BuildServerRpc(payload);
        }

        /// <summary>
        /// Update the desired object for the specified next one.
        /// </summary>
        /// <param name="stateObject">Object to update</param>
        /// <param name="next">The state the object should update to</param>
        public void RequestBuildUpdate(IStateObject stateObject, string nextID)
        {
            int i = GetKeyByValue(stateObject);
            if(-1 == i) {
                Debug.LogWarning($"Cannot update {stateObject} from {stateObject.UID} to {nextID}, not registered on client side.");
            }

            char* cur = StatesManager.Instance.GetSymbol(States[i].UID);
            char* next = StatesManager.Instance.GetSymbol(nextID);
            if(cur == null || next == null){
                Debug.LogWarning($"Cannot update {stateObject} from {States[i].UID} to {nextID}, one of the UID does not exist.");
            }
            BuildUpdatePayload payload = new BuildUpdatePayload(i, *cur, *next);
            UpdateBuildServerRpc(payload);
        }


        /// <summary>
        /// Sends a server rpc to build the selected object.
        /// </summary>
        public void RequestReset()
        {
            if(preview != null) {
                preview.Reset();
            }
        }

        #endregion

        #endregion

        #region Server commands

        /// <summary>
        /// Modify world state according to the id
        /// </summary>
        /// <param name="inputPayload"></param>
        [ServerRpc(RequireOwnership = false)]
        void BuildServerRpc(BuildPayload payload, ServerRpcParams serverRpcParams = default)
        {
            if(!IsServer) return;
            
            BuildableElement elem = catalog[payload.index];

            // TODO(OPEN): Inverse dependancy?
            bool res = StatesManager.Instance.CreateState(elem.UID, payload.transformPayload);
            if(res){
                Debug.Log($"Visual {elem.UID} created at pos: {payload.transformPayload.position}, rot: {payload.transformPayload.rotation}, scale: {payload.transformPayload.scale}");
            }else{
                Debug.Log($"Visual {elem.UID} could not be created.");
            }
        }

        /// <summary>
        /// Modify world state according to the id
        /// </summary>
        /// <param name="inputPayload"></param>
        [ServerRpc(RequireOwnership = false)]
        void UpdateBuildServerRpc(BuildUpdatePayload payload) 
        {
            if(!IsServer) return;

            bool res = StatesManager.Instance.UpdateState(payload.Index, payload.Current, payload.Next);
            if(res){
                Debug.Log($"Visual {payload.Index} updated for: {payload.Next}");
            }else{
                Debug.Log($"Visual {payload.Index} could not be updated.");
            }
        }

        #endregion
    }
}