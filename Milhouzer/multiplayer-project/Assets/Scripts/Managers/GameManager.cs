using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
// using Milhouzer.Utils;
// using Unity.Netcode;
using UnityEngine;
using Milhouzer.Input;
using Milhouzer.BuildingSystem;
using Milhouzer;

public class GameManager : /*NetworkedSingleton<GameManager>*/ MonoBehaviour
{
    [Header("Controller")]
    [SerializeField]
    InputReader input;
    public InputReader Input => input;

    [SerializeField]
    BuildManagerSettings BuildManagerSettings;

    [SerializeField] List<Color> playerColors;

    // private NetworkList<PlayerData> networkPlayersData;
#pragma warning disable CS0067
    public event Action<PlayerData> OnPlayerConnectedCallback;
    public event Action<PlayerData> OnPlayerDisconnectedCallback;
#pragma warning restore CS0067

    /// <summary>
    /// Main camera component, different on each client.
    /// </summary>
    Camera playerCamera;
    
    // void Awake() {
    //     networkPlayersData = new NetworkList<PlayerData>(new List<PlayerData>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // }

    // public override void OnNetworkSpawn()
    // {
    //     base.OnNetworkSpawn();

    //     if(IsServer) 
    //     {
    //         Debug.Log("[GameManager] Init as server");
    //         Shuffle<Color>(ref playerColors);   
    //         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    //         NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    //         networkPlayersData.OnListChanged += OnServerListChanged;
    //         InitServerBuildManager(BuildManagerSettings);
    //     }
    //     else {
    //         Debug.Log("[GameManager] Init as client");
    //         networkPlayersData.OnListChanged += OnClientListChanged;
    //         PlayerController.OnPlayerCreatedCallback += OnPlayerCreatedCallback_Client;
    //         StartCoroutine(CheckPlayerData());
    //     }
    // }

    // /// <summary>
    // /// Init server <see cref="BuildManager"/>, only the catalog is necessary for the server to instantiate objects.
    // /// </summary>
    // /// <param name="settings"></param>
    // void InitServerBuildManager(BuildManagerSettings settings) {
    //     settings = Instantiate(settings);
    //     BuildManager.Instance.Init(settings);
    // }

    // /// <summary>
    // /// Init client <see cref="BuildManager"/>, we pass the player camera and the input used to register callbacks to them
    // /// </summary>
    // /// <remarks>
    // /// Should input be registered in GameManager ?
    // /// </remarks>
    // /// <param name="settings"></param>
    // void InitClientBuildManager(BuildManagerSettings settings) {
    //     settings = Instantiate(settings);
    //     settings.Camera = playerCamera;
    //     BuildManager.Instance.Init(settings);
    //     input.OnEnterBuildModeEvent += BuildManager.Instance.RequestEnterBuildMode;
    //     input.OnExitBuildModeEvent += BuildManager.Instance.RequestExitBuildMode;
    //     input.OnBuildEvent += BuildManager.Instance.RequestBuild;
    //     input.OnRotateEvent += BuildManager.Instance.RequestPreviewRotation;
    //     input.OnScaleEvent += BuildManager.Instance.RequestPreviewScale;
    //     input.OnResetEvent += BuildManager.Instance.RequestReset;
    //     input.OnSelectEvent += BuildManager.Instance.RequestSelect;
    // }

    // /// <summary>
    // /// Player connected callback, init <see cref="BuildManager"/> with data injection
    // /// </summary>
    // /// <param name="controller"></param>
    // private void OnPlayerCreatedCallback_Client(PlayerController controller)
    // {
    //     Debug.Log($"[GameManager] player object created callback on server {controller.CamController.Camera}");
    //     playerCamera = controller.CamController.Camera;
    //     InitClientBuildManager(BuildManagerSettings);
    // }

    // void OnServerListChanged(NetworkListEvent<PlayerData> changeEvent)
    // {
    //     Debug.Log($"[GameManager_Server] The list changed and now has {networkPlayersData.Count} elements on server:");
    //     foreach(PlayerData data in networkPlayersData) {
    //         Debug.Log($"\r\n{data.Username}: {data.Color}");
    //     }
    //     HandlePlayerDataChanged(changeEvent);
    // }

    // void OnClientListChanged(NetworkListEvent<PlayerData> changeEvent)
    // {
    //     if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.Id) return;

    //     Debug.Log($"[GameManager_Client] The list changed and now has {networkPlayersData.Count} elements {this.OwnerClientId}, {changeEvent.Value.Id}");
    //     foreach(PlayerData data in networkPlayersData) {
    //         Debug.Log($"\r\n{data.Username}: {data.Color}");
    //     }
    //     HandlePlayerDataChanged(changeEvent);
    // }

    // private IEnumerator CheckPlayerData()
    // {
    //     yield return new WaitForSeconds(3f);
    //     Debug.Log(networkPlayersData.Count);
    //     foreach (PlayerData data in networkPlayersData)
    //     {
    //         OnPlayerConnectedCallback?.Invoke(data);
    //     }
    // }

    // // Q: How do clients receive event, is it because OnPlayerConnectedCallback is static ?
    // private void HandlePlayerDataChanged(NetworkListEvent<PlayerData> changeEvent)
    // {
    //     PlayerData changed = changeEvent.Value;
    //     Debug.Log($"Handle event change {changeEvent.Type}: {changed.Username}");
    //     switch(changeEvent.Type) {
    //         case NetworkListEvent<PlayerData>.EventType.Add:
    //             OnPlayerConnectedCallback?.Invoke(changed);
    //             return;
    //         case NetworkListEvent<PlayerData>.EventType.Insert:
    //             OnPlayerConnectedCallback?.Invoke(changed);
    //             return;
    //     }
        
    //     OnPlayerDisconnectedCallback?.Invoke(changed);
    // }

    // private void OnClientConnectedCallback(ulong clientId)
    // {
    //     if(!IsServer) 
    //     {
    //         Debug.LogWarning("Tried to call server method from client!");
    //         return;
    //     }

    //     string username = GetUsername();
    //     Color color = playerColors[networkPlayersData.Count];
    //     PlayerData newPlayerData = new PlayerData(clientId, username, color);

    //     networkPlayersData.Add(newPlayerData);

    //     Debug.Log($"Player {username} connected with color {color}");
    // }

    // private void OnClientDisconnectCallback(ulong clientId)
    // {
    //     if(!IsServer) 
    //     {
    //         Debug.LogWarning("Tried to call server method from client!");
    //         return;
    //     }
        
    //     for (int i = networkPlayersData.Count - 1; i >= 0 ; i--)
    //     {
    //         if(networkPlayersData[i].Id == clientId) {
    //             networkPlayersData.RemoveAt(i);
    //             return;
    //         }
    //     }

    //     Debug.Log($"Player {clientId} disconnected");

    // }

    // private static readonly System.Random random = new System.Random();
    // const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    // const int length = 8;

    // /// <summary>
    // /// Random username generator
    // /// </summary>
    // /// <returns></returns>
    // public static string GetUsername()
    // {
    //     StringBuilder result = new StringBuilder(length);

    //     for (int i = 0; i < length; i++)
    //     {
    //         result.Append(chars[random.Next(chars.Length)]);
    //     }

    //     return result.ToString();
    // }

    // /// <summary>
    // /// Utility method to shuffle an array using Fisher-Yates algorithm
    // /// </summary>
    // /// <param name="list"></param>
    // /// <typeparam name="T"></typeparam>
    // public static void Shuffle<T>(ref List<T> list)
    // {
    //     System.Random rng = new System.Random();
    //     int n = list.Count;
    //     while (n > 1)
    //     {
    //         n--;
    //         int k = rng.Next(n + 1);
    //         T value = list[k];
    //         list[k] = list[n];
    //         list[n] = value;
    //     }
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void SelectServerRpc(NetworkObjectReference selectableRef, ulong requestingClientId)
    // {
    //     Debug.Log($"{requestingClientId} tried to select {selectableRef.NetworkObjectId}");
    //     if (selectableRef.TryGet(out NetworkObject selectableObject))
    //     {
    //         ISelectable selectable = selectableObject.GetComponent<ISelectable>();
    //         if (selectable != null && !selectable.IsSelected())
    //         {
    //             selectable.Select(requestingClientId);
    //             // NotifyClientsSelectionClientRpc(selectableRef, requestingClientId);
    //         }else {
    //             Debug.Log($"selectable {selectable} is selected: {selectable.IsSelected()}");
    //         }
    //     }else {
    //         Debug.Log($"could not get NetworkObject out of {selectableRef.NetworkObjectId}");
    //     }
    // }

    // /// <summary>
    // /// Try get player data based on a given client Id.
    // /// </summary>
    // /// <param name="clientId"></param>
    // /// <param name="data"></param>
    // /// <returns></returns>
    // public bool TryGetPlayerData(ulong clientId, out PlayerData data) {
    //     foreach(PlayerData playerData in networkPlayersData) {
    //         data = playerData;
    //         return true;
    //     }

    //     data = default;
    //     return false;
    // }
}
