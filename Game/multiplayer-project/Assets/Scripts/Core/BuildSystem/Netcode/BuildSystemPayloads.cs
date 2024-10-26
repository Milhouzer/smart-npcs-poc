using System;
using UnityEngine;
using Unity.Netcode;

namespace Milhouzer.Core.BuildSystem 
{
    /// <summary>
    /// BuildPayload contains information to pass on the network that will be passed in the build server rpc :
    /// - index of the object to build in the catalog
    /// - a <see cref="TransformPayload"/>
    /// </summary>
    public struct BuildPayload : INetworkSerializable
    {
        public int index;
        public TransformPayload transformPayload;

        public BuildPayload(int i, Vector3 pos, Quaternion rot, Vector3 s)
        {
            index = i;
            transformPayload = new TransformPayload(pos, rot, s);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref index);
            serializer.SerializeValue(ref transformPayload);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BuildUpdatePayload : INetworkSerializable
    {
        public int Index;
        public char Current;
        public char Next;

        public BuildUpdatePayload(int i, char cur, char next)
        {
            Index = i;
            Current = cur;
            Next = next;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Index);
            serializer.SerializeValue(ref Current);
            serializer.SerializeValue(ref Next);
        }
    }

    /// <summary>
    /// TransformPayload is used to serialize transform information over the network :
    /// - position, rotation and scale of the object
    /// </summary>
    public struct TransformPayload : INetworkSerializable, IEquatable<TransformPayload>
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformPayload(Vector3 pos, Quaternion rot, Vector3 s)
        {
            position = pos;
            rotation = rot;
            scale = s;
        }

        public bool Equals(TransformPayload other)
        {
            return other.position == position && other.rotation == rotation && other.scale == scale;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref scale);
        }   
    }
}