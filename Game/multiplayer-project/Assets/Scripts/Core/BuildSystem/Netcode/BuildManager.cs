using System;
using System.Collections.Generic;
using System.Linq;
using Milhouzer.Core.BuildSystem.StatesManagement;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Milhouzer.Core.BuildSystem
{
    /// <summary>
    /// PreviewSettings holds visualisation and technical settings: preview model material, collision layer, etc.
    /// </summary>
    [Serializable]
    public class PreviewSettings {
        public Material Material;
        public int Layer = 10;
    }
    
    /// <summary>
    /// BuildManager handles the building system, objects are previewed locally, a ServerRpc is sent to spawn objects on the network.
    /// </summary>
    public class BuildManager : NetworkedSingleton<BuildManager>, IManager<BuildManagerSettings>
    {
        #region Attributes

        /// <summary>
        /// Objects that can be built.
        /// </summary>
        [SerializeField] private BuildableCatalog catalog;

        /// <summary>
        /// Catalog accessor
        /// </summary>
        public BuildableCatalog Catalog => catalog;

        /// <summary>
        /// Preview settings.
        /// </summary>
        [SerializeField]private PreviewSettings previewSettings;

        /// <summary>
        /// Camera used to raycast for preview placement.
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// Preview handler used to preview buildings in the world.
        /// </summary>
        private BuilderPreview _preview;

        /// <summary>
        /// Current selected buildable.
        /// </summary>
        private int _selectedBuildable = 0;

        /// <summary>
        /// <see cref="_selectedBuildable"/> accessor.
        /// </summary>
        public int Selected =>  _selectedBuildable % catalog.Count();

        /// <summary>
        /// Marker to indicate the selection index changed
        /// </summary>
        private Timer _updatePreviewTimer;

        /// <summary>
        /// Client-side saved states to represent 3d objects.
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<int, IStateObject> _states = new();
        
        [SerializeField]
        private StatesManager _statesManager;
        
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
        }

        /// <summary>
        /// Init method, register callbacks for client and initialize objects catalog on both client and server.
        /// </summary>
        /// <param name="settings"></param>
        public void InitClientManager(BuildManagerSettings settings)
        {
            if (!IsClient) return;

            _statesManager.SetNamespace(settings.Namespace);
            
            catalog = settings.Catalog;
            previewSettings = settings.PreviewSettings;
            _updatePreviewTimer = new Timer(0.3f, UpdatePreviewObject);
            SyncVisuals();
            _statesManager.ReplicatedStates.OnListChanged += OnClientStatesChanged;
        }

        public void InitServerManager(BuildManagerSettings settings)
        {
            if (!IsServer) return;
            
            _statesManager.SetNamespace(settings.Namespace);
        }

        #region Visuals management


        /// <summary>
        /// Method called when the client is initialized. Synchronizes all visuals based on synced states
        /// </summary>
        private void SyncVisuals()
        {
            if(IsServer) return;

            int index = 0;
            foreach (ReplicatedStateInformation rs in _statesManager.ReplicatedStates)
            {
                BuildVisual(rs.State, index, rs.Transform);
            }
        }


        /// <summary>
        /// Callback called when the server states have changed
        /// </summary>
        /// <param name="e"></param>
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
        /// </summary>4
        /// <param name="index"></param>
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
            string uid = _statesManager.GetUid(c);

            if(!catalog.Get(x => x.UID == uid, out BuildableElement elem)) {
                Debug.Log($"{uid}:{c} does not exist in catalog");
                return;
            }

            Debug.Log($"BuildVisual {c}:{uid} at index {index}, transform {tr}");

            GameObject visual = elem.Object.Build(tr.position, tr.rotation, tr.scale);
            if(!visual.TryGetComponent<IStateObject>(out var holder)) {
                Debug.LogError($"State holder missing on {visual}, misconfiguration");
                return;
            }

            holder.Uid = uid;
            if(_states.TryGetValue(index, out IStateObject existing))
            {
                existing.Destroy();
                _states[index] = holder;
                return;
            }

            _states.Add(index, holder);
        }

        int GetKeyByValue(IStateObject value)
        {
            foreach (var kvp in _states)
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
            _preview = new BuilderPreview(catalog[_selectedBuildable], previewSettings);

            // TODO(OPEN): Move to UI component ??
            _updatePreviewTimer.Restart();
            OnEnterBuildMode?.Invoke();
        }

        /// <summary>
        /// Exit build mode input callback
        /// </summary>
        public void RequestExitBuildMode() {

            Debug.Log("[BuildManager] exit building mode");
            if (_preview != null) {
                _preview.Cleanup();
                _preview = null;
            }

            // TODO(OPEN): Move to UI component
            _updatePreviewTimer.Stop();
            OnExitBuildMode?.Invoke();
        }

        /// <summary>
        /// Request selection change on the catalog
        /// </summary>
        /// <param name="amount"></param>
        internal void RequestSelect(int amount)
        {
            _selectedBuildable = (_selectedBuildable + amount % catalog.Count() + catalog.Count()) % catalog.Count();
            NotifyIndexChanged_Internal();
        }

        /// <summary>
        /// Notify that the selection has changed, reset the timer to update the preview.
        /// </summary>
        private void NotifyIndexChanged_Internal()
        {
            _updatePreviewTimer.Restart();
            OnBuildSelectionChange?.Invoke(Selected);
        }

        /// <summary>
        /// Preview the object at the specified position, rotation and scale
        /// </summary>
        /// <param name="pos"></param>
        public void Preview(Vector3 pos) {
            if(_preview == null) {
                Debug.LogWarning("preview is null");
                return;
            }

            if(_updatePreviewTimer.IsRunning){
                _updatePreviewTimer.Update(Time.deltaTime);
            } 
            _preview.Preview(pos, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePreviewObject() {
            Debug.Log("[BuildManager] update preview for index " + _selectedBuildable);
            _preview.Cleanup();
            _preview = new BuilderPreview(catalog[Mathf.Abs(_selectedBuildable)], previewSettings);
            _updatePreviewTimer.Stop();
        }

        /// <summary>
        /// Input handle to rotate 
        /// </summary>
        public void RequestPreviewRotation(Vector2 amount) {
            _preview.Rotate(amount.y);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestPreviewScale(Vector2 amount) {
            _preview.Scale(amount.y);
        }

        /// <summary>
        /// Sends a server rpc to build the selected object.
        /// </summary>
        public void RequestBuild()
        {
            BuildPayload payload = new BuildPayload(
                _selectedBuildable,
                _preview.Position,
                _preview.Rotation,
                _preview.LocalScale

            );
            BuildServerRpc(payload);
        }

        /// <summary>
        /// Update the desired object for the specified next one.
        /// </summary>
        /// <param name="stateObject">Object to update</param>
        /// <param name="nextID">The state the object should update to</param>
        public void RequestBuildUpdate(IStateObject stateObject, string nextID)
        {
            int i = GetKeyByValue(stateObject);
            if(-1 == i) {
                Debug.LogWarning($"Cannot update {stateObject} from {stateObject.Uid} to {nextID}, not registered on client side.");
                return;
            }

            char cur = _statesManager.GetSymbol(_states[i].Uid);
            char next = _statesManager.GetSymbol(nextID);
            if(cur == char.MaxValue || next == char.MaxValue){
                Debug.LogWarning($"Cannot update {stateObject} from {_states[i].Uid} to {nextID}, one of the UID does not exist.");
                return;
            }
            Debug.Log($"[BuildManager] Request {stateObject} update for {nextID}: cur={cur} and next={next}");
            BuildUpdatePayload payload = new BuildUpdatePayload(i, cur, next);
            UpdateBuildServerRpc(payload);
        }


        /// <summary>
        /// Sends a server rpc to build the selected object.
        /// </summary>
        public void RequestReset()
        {
            if(_preview != null) {
                _preview.Reset();
            }
        }

        #endregion

        #endregion

        #region Server commands

        /// <summary>
        /// Modify world state according to the id
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="serverRpcParams"></param>
        [ServerRpc(RequireOwnership = false)]
        void BuildServerRpc(BuildPayload payload, ServerRpcParams serverRpcParams = default)
        {
            if(!IsServer) return;
            
            BuildableElement elem = catalog[payload.index];

            // TODO(OPEN): Inverse dependency?
            Debug.Log(_statesManager.CreateState(elem.UID, payload.transformPayload)
                ? $"Visual {elem.UID} created at pos: {payload.transformPayload.position}, rot: {payload.transformPayload.rotation}, scale: {payload.transformPayload.scale}"
                : $"Visual {elem.UID} could not be created.");
        }

        /// <summary>
        /// Modify world state according to the id
        /// </summary>
        /// <param name="payload"></param>
        [ServerRpc(RequireOwnership = false)]
        private void UpdateBuildServerRpc(BuildUpdatePayload payload)
        {
            if(!IsServer) return;

            Debug.Log(_statesManager.UpdateState(payload.Index, payload.Current, payload.Next)
                ? $"Visual {payload.Index} updated for: {payload.Next}"
                : $"Visual {payload.Index} could not be updated.");
        }

        #endregion
    }
}