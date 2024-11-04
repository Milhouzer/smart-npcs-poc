using System;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Milhouzer.UI
{
    public class GameStartUI : NetworkBehaviour
    {
        [Header("Connection buttons")]
        [SerializeField] Button clientBtn;
        [Tooltip("Debug only.")]
        [SerializeField] Button serverBtn;
        
        [Header("Connection parameters")]
        [SerializeField] TextMeshProUGUI usernameText;
        [SerializeField] TextMeshProUGUI passwordText;
        [SerializeField] private TextMeshProUGUI serverIP;
        [SerializeField] private TextMeshProUGUI serverPort;

        private string _serverIP;
        private ushort _serverPort;
        
        private void Start() {
            clientBtn.onClick.AddListener(() => {
                // if (!ValidateRequirements())
                // {
                //     return;
                // }

                // NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                SceneManager.LoadScene("GameScene");
            });
            
            serverBtn.onClick.AddListener(() =>
            {
                // if (ValidateRequirements())
                // {
                    NetworkManager.Singleton.StartServer();
                // }
            });
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            }
        }

        // private void OnClientConnected(ulong clientId)
        // {
        //     if (!NetworkManager.Singleton.IsClient || clientId != NetworkManager.Singleton.LocalClientId) return;
        //     
        //     NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        //
        //     // TODO: Load scene async, make utility script to pass callback in method.
        //     UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        // }

        private bool ValidateRequirements()
        {
            // if (!IPAddress.TryParse(serverIP.text, out var ip))
            // {
            //     Debug.LogError("Invalid IP address format {serverIP.text}");
            //     return false;
            // }
            //

            _serverIP = serverIP.text;
            if (!ushort.TryParse(serverPort.text, out ushort port))
            {
                Debug.LogError($"Invalid port number {port}/{serverPort.text}");
                return false;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("UnityTransport component not found on NetworkManager.");
                return false;
            }

            transport.ConnectionData.Address = "127.0.0.1";
            transport.ConnectionData.Port = 7777;

            return true;
        }
    }
}
