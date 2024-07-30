using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] List<Color> playerColors;

    private NetworkList<PlayerData> networkPlayersData;

    // Try to remove static to see how it behaves.
    public event Action<PlayerData> OnPlayerConnectedCallback;
    public event Action<PlayerData> OnPlayerDisconnectedCallback;

    void Awake() {
        networkPlayersData = new NetworkList<PlayerData>(new List<PlayerData>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if(IsServer) 
        {         
            Shuffle<Color>(ref playerColors);   
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            networkPlayersData.OnListChanged += OnServerListChanged;
        }
        else {
            networkPlayersData.OnListChanged += OnClientListChanged;
            StartCoroutine(CheckPlayerData());
        }
    }

    void OnServerListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        Debug.Log($"[S] The list changed and now has {networkPlayersData.Count} elements:");
        foreach(PlayerData data in networkPlayersData) {
            Debug.Log($"\r\n{data.Username}: {data.Color}");
        }
        HandlePlayerDataChanged(changeEvent);
    }

    void OnClientListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.Id) return;

        Debug.Log($"[C] The list changed and now has {networkPlayersData.Count} elements {this.OwnerClientId}, {changeEvent.Value.Id}");
        foreach(PlayerData data in networkPlayersData) {
            Debug.Log($"\r\n{data.Username}: {data.Color}");
        }
        HandlePlayerDataChanged(changeEvent);
    }

    private IEnumerator CheckPlayerData()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log(networkPlayersData.Count);
        foreach (PlayerData data in networkPlayersData)
        {
            OnPlayerConnectedCallback?.Invoke(data);
        }
    }

    // Q: How do clients receive event, is it because OnPlayerConnectedCallback is static ?
    private void HandlePlayerDataChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        PlayerData changed = changeEvent.Value;
        Debug.Log($"Handle event change {changeEvent.Type}: {changed.Username}");
        switch(changeEvent.Type) {
            case NetworkListEvent<PlayerData>.EventType.Add:
                OnPlayerConnectedCallback?.Invoke(changed);
                return;
            case NetworkListEvent<PlayerData>.EventType.Insert:
                OnPlayerConnectedCallback?.Invoke(changed);
                return;
        }
        
        OnPlayerDisconnectedCallback?.Invoke(changed);
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if(!IsServer) 
        {
            Debug.LogWarning("Tried to call server method from client!");
            return;
        }

        string username = GetUsername();
        Color color = playerColors[networkPlayersData.Count];
        PlayerData newPlayerData = new PlayerData(clientId, username, color);

        networkPlayersData.Add(newPlayerData);

        Debug.Log($"Player {username} connected with color {color}");
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if(!IsServer) 
        {
            Debug.LogWarning("Tried to call server method from client!");
            return;
        }
        
        for (int i = networkPlayersData.Count - 1; i >= 0 ; i--)
        {
            if(networkPlayersData[i].Id == clientId) {
                networkPlayersData.RemoveAt(i);
                return;
            }
        }

        Debug.Log($"Player {clientId} disconnected");

    }

    private static readonly System.Random random = new System.Random();
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    const int length = 8;

    public static string GetUsername()
    {
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
