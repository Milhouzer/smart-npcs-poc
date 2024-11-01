using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.Core.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        [SerializeField] 
        
        private List<Color> playerColors;
        
        private NetworkList<PlayerData> _networkPlayersData;

        public delegate void OwnerConnectedEvent();
        public delegate void PlayerConnectedEvent(PlayerData data);
        public delegate void PlayerDisconnectedEvent(PlayerData data);
        public static event OwnerConnectedEvent OnOwnerConnected;
        public static event PlayerConnectedEvent OnPlayerConnected;
        public static event PlayerDisconnectedEvent OnPlayerDisconnected;
        
        private void Awake() {
            _networkPlayersData = new NetworkList<PlayerData>(new List<PlayerData>(), 
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Server);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            var role = IsServer ? "Server" : "Client";
            Debug.Log($"[PlayersManager] Init {role} player manager");
            if(IsClient) InitClient();
            if(IsServer) InitServer();

        }

        private void InitClient()
        {
            if (!IsClient) return;
            
            _networkPlayersData.OnListChanged += OnClientListChanged;

            if (IsOwner) InitOwner();
        }

        private void InitOwner()
        {
            OnOwnerConnected?.Invoke();
        }

        private void InitServer()
        {
            if (!IsServer) return;
            
            Utility.Shuffle(ref playerColors);   
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            _networkPlayersData.OnListChanged += OnServerListChanged;
        }
        
        private void OnClientConnectedCallback(ulong clientId)
        {
            if(!IsServer) 
            {
                Debug.LogWarning("Tried to call server method from client!");
                return;
            }

            string username = Utility.GetRandomUsername();
            Color color = playerColors[_networkPlayersData.Count];
            PlayerData newPlayerData = new PlayerData(clientId, username, color);

            _networkPlayersData.Add(newPlayerData);

            Debug.Log($"Player {username} connected with color {color}");
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if(!IsServer) 
            {
                Debug.LogWarning("Tried to call server method from client!");
                return;
            }
            
            for (int i = _networkPlayersData.Count - 1; i >= 0 ; i--)
            {
                if(_networkPlayersData[i].Id == clientId) {
                    _networkPlayersData.RemoveAt(i);
                    return;
                }
            }

            Debug.Log($"Player {clientId} disconnected");

        }
        
        void OnServerListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            Debug.Log($"[GameManager_Server] The list changed and now has {_networkPlayersData.Count} elements on server:");
            foreach(PlayerData data in _networkPlayersData) {
                Debug.Log($"\r\n{data.Username}: {data.Color}");
            }
            HandlePlayersListChanged(changeEvent);
        }

        void OnClientListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.Id) return;

            Debug.Log($"[GameManager_Client] The list changed and now has {_networkPlayersData.Count} elements {this.OwnerClientId}, {changeEvent.Value.Id}");
            foreach(PlayerData data in _networkPlayersData) {
                Debug.Log($"\r\n{data.Username}: {data.Color}");
            }
            HandlePlayersListChanged(changeEvent);
        }
        
        // Q: How do clients receive event, is it because OnPlayerConnectedCallback is static ?
        private void HandlePlayersListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            Debug.Log($"Handle event change {changeEvent.Type}: {changeEvent.Value.Username}");
            switch(changeEvent.Type) {
                case NetworkListEvent<PlayerData>.EventType.Add:
                    OnPlayerConnected?.Invoke(changeEvent.Value);
                    return;
                case NetworkListEvent<PlayerData>.EventType.Insert:
                    OnPlayerConnected?.Invoke(changeEvent.Value);
                    return;
                case NetworkListEvent<PlayerData>.EventType.Remove:
                    OnPlayerDisconnected?.Invoke(changeEvent.Value);
                    return;
                case NetworkListEvent<PlayerData>.EventType.RemoveAt:
                    OnPlayerDisconnected?.Invoke(changeEvent.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}