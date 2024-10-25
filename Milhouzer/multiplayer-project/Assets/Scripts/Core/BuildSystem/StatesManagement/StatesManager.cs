using System;
using System.Collections.Generic;
using Milhouzer.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem.StatesManagement
{

    /// <summary>
    /// ReplicatedStateInformation represents the information of a state for a networked object.
    /// </summary>
    public struct ReplicatedStateInformation : INetworkSerializable, System.IEquatable<ReplicatedStateInformation>
    {
        public char State;
        public TransformPayload Transform;

        public ReplicatedStateInformation(char c, Vector3 pos, Quaternion rot, Vector3 s)
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
    public unsafe class StatesManager : NetworkedSingleton<StatesManager>
    {
        #region Shared logic
        [SerializeField]
        StatesNamespace Namespace;

        public NetworkList<ReplicatedStateInformation> ReplicatedStates;
        List<string> InternalServerStates = new();
        
        /// <summary>
        /// Initialize 
        /// </summary>
        private void Awake()
        {
            ReplicatedStates = new NetworkList<ReplicatedStateInformation>();
            Namespace = Instantiate(Namespace);
            Namespace.PrintNodes();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsServer)
            {
                // TODO(IMPROVEMENT): Load save, do server logic.
            }
        }

        internal string GetUID(char c)
        {
            Node node = Namespace.GetNode(c);
            if(node.Equals(default(Node))){
                Debug.LogWarning($"Node {c} is null");
                return "";
            }

            return node.UID;
        }

        internal char GetSymbol(string UID)
        {
            Node node = Namespace.GetNode(UID);
            if(node.Equals(default(Node))){
                Debug.LogWarning($"Node {UID} is null");
                return char.MaxValue;
            }

            return node.Symbol;
        }

        #endregion

        #region Server logic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public bool CreateState(string root, TransformPayload tr)
        {
            if(!IsServer) return false;

            Node rootNode = Namespace.GetNode(root);
            if(rootNode.Equals(default(Node))){
                Debug.LogWarning($"Root node {root} is null");
                return false;
            }
            
            ReplicatedStateInformation s = new ReplicatedStateInformation()
            {
                State = rootNode.Symbol,
                Transform = tr,
            };
            ReplicatedStates.Add(s);
            Debug.Log($"Created {root}/{rootNode.Symbol} (symbol {rootNode.Symbol}) at {tr}");
            InternalServerStates.Add(rootNode.UID);

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
            if(!IsServer) return false;

            Node currentNode = Namespace.GetNode(InternalServerStates[i]);
            if(currentNode.Equals(default(Node))){
                Debug.LogWarning($"Node {InternalServerStates[i]} is null");
                return false;
            }

            if(currentNode.Symbol != cur) {
                Debug.LogWarning($"Node {InternalServerStates[i]} cannot be updated, cur and symbol are different: {InternalServerStates[i]}/{currentNode.Symbol}, {cur}");
                return false;
            }

            Node nextNode = currentNode.GetChild(next);
            if(currentNode.Equals(default(Node))){
                Debug.LogWarning($"Node {InternalServerStates[i]} has no child {next}");
                return false;
            }

            ReplicatedStateInformation s = new ReplicatedStateInformation()
            {
                State = next,
                // TODO(BUG): Transform seems to not be set correctly.
                Transform = ReplicatedStates[i].Transform,
            };
            Debug.Log($"Updated state {i},{cur},{next}: {ReplicatedStates[i].State} ({InternalServerStates[i]}) for {s.State} ({nextNode.UID})");
            ReplicatedStates[i] = s;
            InternalServerStates[i] = nextNode.UID;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool DeleteState(int i)
        {
            if(!IsServer) return false;

            ReplicatedStates.RemoveAt(i);

            return true;
        }

        #endregion
    } 
}
