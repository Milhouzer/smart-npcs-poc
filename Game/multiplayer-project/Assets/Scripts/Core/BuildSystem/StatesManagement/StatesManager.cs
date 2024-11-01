using System;
using System.Collections.Generic;
using Milhouzer.Utils;
using Unity.Netcode;
using Unity.Mathematics;
using Utils;

namespace Milhouzer.Core.BuildSystem.StatesManagement
{

    /// <summary>
    /// ReplicatedStateInformation represents the information of a state for a networked object.
    /// </summary>
    public struct ReplicatedStateInformation : INetworkSerializable, System.IEquatable<ReplicatedStateInformation>
    {
        public char State;
        public TransformPayload Transform;

        public ReplicatedStateInformation(char c, float3 pos, quaternion rot, float3 s)
        {
            State = c;
            Transform = new TransformPayload(pos, rot, s);
        }

        public bool Equals(ReplicatedStateInformation other)
        {
            throw new System.NotImplementedException();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref State);
            serializer.SerializeValue(ref Transform);
        }
    }

    /// <summary>
    /// Networked states manager.
    /// This is basically a CRUD.
    /// </summary>
    [GenerateSerializationForType(typeof(byte))]
    public class StatesManager
    {
        StatesNamespace Namespace;

        public NetworkList<ReplicatedStateInformation> ReplicatedStates;
        readonly List<string> _internalServerStates = new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        public StatesManager(StatesNamespace ns)
        {
            ReplicatedStates = new NetworkList<ReplicatedStateInformation>();
            Namespace = ns;
            Namespace.PrintNodes();
        }

        internal string GetUid(char c)
        {
            Node node = Namespace.GetNode(c);
            if(node.Equals(default(Node))){
                // Debug.LogWarning($"Node {c} is null");
                return "";
            }

            return node.UID;
        }

        internal char GetSymbol(string uid)
        {
            Node node = Namespace.GetNode(uid);
            if(node.Equals(default(Node))){
                // Debug.LogWarning($"Node {UID} is null");
                return char.MaxValue;
            }

            return node.Symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public bool CreateState(string root, TransformPayload tr)
        {
            Node rootNode = Namespace.GetNode(root);
            if(rootNode.Equals(default(Node))){
                // Debug.LogWarning($"Root node {root} is null");
                return false;
            }
            
            ReplicatedStateInformation s = new ReplicatedStateInformation()
            {
                State = rootNode.Symbol,
                Transform = tr,
            };
            ReplicatedStates.Add(s);
            //Debug.Log($"Created {root}/{rootNode.Symbol} (symbol {rootNode.Symbol}) at {tr}");
            _internalServerStates.Add(rootNode.UID);

            return true;
        }

        /// <summary>
        /// Update the state with i state cur for state next
        /// </summary>
        /// <param name="i">the state to update</param>
        /// <param name="cur">the current state according to the caller</param>
        /// <param name="next">the next state to update</param>
        /// <returns></returns>
        public bool UpdateState(int i, char cur, char next)
        {
            Node currentNode = Namespace.GetNode(_internalServerStates[i]);
            if(currentNode.Equals(default(Node))){
                // Debug.LogWarning($"Node {InternalServerStates[i]} is null");
                return false;
            }

            if(currentNode.Symbol != cur) {
                // Debug.LogWarning($"Node {InternalServerStates[i]} cannot be updated, cur and symbol are different: {InternalServerStates[i]}/{currentNode.Symbol}, {cur}");
                return false;
            }

            Node nextNode = currentNode.GetChild(next);
            if(currentNode.Equals(default(Node))){
                // Debug.LogWarning($"Node {InternalServerStates[i]} has no child {next}");
                return false;
            }

            ReplicatedStateInformation s = new ReplicatedStateInformation()
            {
                State = next,
                // TODO(BUG): Transform seems to not be set correctly.
                Transform = ReplicatedStates[i].Transform,
            };
            // Debug.Log($"Updated state {i},{cur},{next}: {ReplicatedStates[i].State} ({InternalServerStates[i]}) for {s.State} ({nextNode.UID})");
            ReplicatedStates[i] = s;
            _internalServerStates[i] = nextNode.UID;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool DeleteState(int i)
        {
            ReplicatedStates.RemoveAt(i);

            return true;
        }
    }
}
