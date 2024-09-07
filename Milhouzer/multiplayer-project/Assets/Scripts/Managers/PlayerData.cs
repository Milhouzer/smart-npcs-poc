using System;
using Unity.Collections;
// using Unity.Netcode;
using UnityEngine;


[Serializable]
public struct PlayerData : /*INetworkSerializable,*/ IEquatable<PlayerData>
{
    public ulong _clientId;
    public ulong Id => _clientId;
    private FixedString64Bytes _username;
    public string Username => _username.ToString();
    
    private Color32 _color;
    public Color32 Color => _color;

    public PlayerData( ulong id, string username, Color32 color)
    {
        _clientId = id;
        _username = username;
        _color = color;
    }


    public bool Equals(PlayerData other)
    {
        return other.Username == Username;
    }

    // public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    // {
    //     serializer.SerializeValue(ref _clientId);
    //     serializer.SerializeValue(ref _username);
    //     serializer.SerializeValue(ref _color);
    // }
}