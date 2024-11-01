using System;
using System.Collections.Generic;
using Milhouzer.Core.BuildSystem.ContextActions;
using Milhouzer.Core.Player;
using Milhouzer.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using UnityEngine.UI;

namespace Milhouzer.UI
{
    public class UIManager : Singleton<UIManager>, IManager<UIManagerSettings>
    {
        private ContextAction _heldAction; 
        /// <summary>
        /// 
        /// </summary>
        // TODO(improve): add HeldActionChange event
        public ContextAction HeldAction {
            get => _heldAction;
            set
            {
                OnHeldActionChanged?.Invoke(_heldAction, value);
                _heldAction = value; 
            }
        }
        public delegate void HeldActionChanged(ContextAction old, ContextAction newAction);
        public event HeldActionChanged OnHeldActionChanged;
        
        /// <summary>
        /// Camera used to raycast from mouse when executing action.
        /// </summary>
        private Camera _camera;

        private void Awake()
        {
            PlayerManager.OnPlayerConnected += PlayerConnected;
            PlayerManager.OnPlayerDisconnected += PlayerDisconnected;
        }

        [SerializeField] Transform players;
        [SerializeField] PlayerCard playerCard;

        Dictionary<string, PlayerCard> cards = new();
        
        private void OwnerConnected()
        {
            throw new NotImplementedException();
        }
        
        private void PlayerConnected(PlayerData playerData)
        {
            PlayerCard card = Instantiate(playerCard, players, true);

            // Pimp card
            card.SetPlayer(playerData);
            card.GetComponent<Image>().color = playerData.Color;
            card.name = playerData.Username;
            card.gameObject.SetActive(true);

            cards.Add(playerData.Username, card);
            
        }

        private void PlayerDisconnected(PlayerData data)
        {
            if (!cards.TryGetValue(data.Username, out PlayerCard card)) return;
            
            Destroy(card.gameObject);
            cards.Remove(data.Username);
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        /// <summary>
        /// Set camera for ray casting
        /// </summary>
        /// <param name="settings"></param>
        public void InitClientManager(UIManagerSettings settings)
        {
            _camera = settings.Camera;
            if(_camera) _camera = _camera;
        }

        /// <summary>
        /// Destroy ui manager on server
        /// </summary>
        /// <param name="settings"></param>
        public void InitServerManager(UIManagerSettings settings)
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Execute the current held action on the targeted object by the mouse if not null.
        /// <see cref="HeldAction"/> becomes null after this method.
        /// </summary>
        /// <returns></returns>
        internal bool ExecuteContextAction()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("[UIManager] Cursor is over UI, canceling action.");
                HeldAction = null;
                return true;
            }

            if(HeldAction == null) return false;
            
            Debug.Log($"[UIManager] Use context action {HeldAction}");
            if (Physics.Raycast(_camera.ScreenPointToRay(UnityEngine.Input.mousePosition), out var hit, 150f, ~(1 << 10)))
            {
                if(!hit.transform.gameObject.TryGetComponent<IContextActionHandler>(out var handler)) {
                    Debug.Log("[UIManager] No handler detected to execute context action");
                    HeldAction = null;
                    return true;
                }
                HeldAction.Execute(handler);
                HeldAction = null;
                return true;
            }

            HeldAction = null;
            return false;
        }
    }
}
