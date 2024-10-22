using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem.StatesManagement
{
    [System.Serializable]
    public struct Node 
    { 
        /// <summary>
        /// Encoded symbol. Symbols must be unique in a tree
        /// </summary>
        public char Symbol;

        /// <summary>
        /// Human readable id
        /// </summary>
        public string UID;

        /// <summary>
        /// Child nodes
        /// </summary>
        public List<Node> Child;
        
        /// <summary>
        /// Get child node with <see cref="Symbol">
        /// </summary>
        /// <param name="c">Symbol of the child node</param>
        /// <returns>
        /// Node if such a node exists, null otherwise
        /// </returns>
        public Node GetChild(char c)
        {
            return Child.FirstOrDefault(x => x.Symbol == c);
        }
    }

    /// <summary>
    /// StatesNamespace holds a list of <see cref="Node"/>
    /// </summary>
    /// <remarks>
    /// The data structure (A tree) used is not optimal for this.
    /// </remarks>
    [CreateAssetMenu(fileName = "StatesNamespace", menuName = "Builder/StatesNamespace", order = 0)]
    public class StatesNamespace : ScriptableObject {

        [SerializeField]
        public List<Node> Nodes;

        public Node GetRoot(string root)
        {
            return Nodes.FirstOrDefault(x => x.UID == root);
        }

        public void PrintNodes() {
            foreach(Node node in Nodes)
            {
                Debug.Log($"Node: {node.Symbol}:{node.UID}, child count: {node.Child.Count()}");
                
            }
        }

        public Node GetNode(char symbol)
        {
            foreach (var node in Nodes)
            {
                var result = FindNodeDFS(node, symbol);
                if (!result.Equals(default(Node))) return result; 
            }

            return default(Node);
        }

        public Node GetNode(string UID)
        {
            foreach (var node in Nodes)
            {
                var result = FindNodeDFS(node, UID);
                if (!result.Equals(default(Node))) return result; 
            }

            return default(Node);
        }

        private Node FindNodeDFS(Node node, char symbol)
        {
            if (node.Symbol == symbol) return node;

            foreach (var child in node.Child)
            {
                var result = FindNodeDFS(child, symbol);
                Debug.Log($"Find node DFS for {symbol}: {result.UID}:{result.Symbol}");
                if (result.Symbol == symbol) return result;
            }

            return default(Node);
        }


        private Node FindNodeDFS(Node node, string UID)
        {
            if (node.UID == UID) return node;

            foreach (var child in node.Child)
            {
                var result = FindNodeDFS(child, UID);
                Debug.Log($"Find node DFS for {UID}: {result.UID}:{result.Symbol}");
                if (result.UID == UID) return result;
            }

            return default(Node);
        }

        public List<Node> GetChild(string UID) 
        {
            Node node = GetNode(UID);
            if(!Object.ReferenceEquals(node, default)) return null;

            return node.Child;
        }
    }
}