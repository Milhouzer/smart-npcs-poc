using System;
using System.Collections.Generic;
using Milhouzer.Core.BuildSystem.ContextActions;
using Milhouzer.Core.BuildSystem.UI;
using Milhouzer.Core.Player;
using Milhouzer.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;
using UnityEngine.UI;

namespace Milhouzer.UI
{
    public class UIManager : Singleton<UIManager>, IManager<UIManagerSettings>
    {
        [SerializeField] private BuilderUI builderUI;
        
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

        private void Start()
        {
            if(!_camera) _camera = Camera.main;

            SyncPlayerCards();
        }
        
        private void OnDestroy()
        {
            PlayerManager.OnPlayerConnected -= PlayerConnected;
            PlayerManager.OnPlayerDisconnected -= PlayerDisconnected;
        }


        /// <summary>
        /// Set camera for ray casting
        /// </summary>
        /// <param name="settings"></param>
        public void InitClientManager(UIManagerSettings settings)
        {
            if(settings.Camera) _camera = settings.Camera;
            
            PlayerManager.OnPlayerConnected += PlayerConnected;
            PlayerManager.OnPlayerDisconnected += PlayerDisconnected;

            if (builderUI != null)
            {
                builderUI.Init();
            }
        }

        /// <summary>
        /// throw if server is loading a UI manger
        /// </summary>
        /// <param name="settings"></param>
        public void InitServerManager(UIManagerSettings settings)
        {
            throw new UnityException("UIManager should not init on server side");
        }

        [SerializeField] Transform players;
        [SerializeField] PlayerCard playerCard;

        Dictionary<string, PlayerCard> cards = new();

        private void SyncPlayerCards()
        {
            foreach (var data in PlayerManager.Instance.GetPlayersData())
            {
                CreatePlayerCard(data);
            }
        }
        
        private void PlayerConnected(PlayerData playerData)
        {
            CreatePlayerCard(playerData);
        }

        private void CreatePlayerCard(PlayerData playerData)
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
