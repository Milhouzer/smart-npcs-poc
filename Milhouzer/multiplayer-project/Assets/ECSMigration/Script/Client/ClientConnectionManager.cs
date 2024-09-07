using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Milhouzer.Netcode
{
    public struct ClientConnectionRequest : IComponentData
    {
        public FixedString32Bytes Room;
    }

    public class ClientConnectionManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _addressField;
        [SerializeField] private TMP_InputField _portField;
        [SerializeField] private TMP_InputField _room;
        [SerializeField] private TMP_Dropdown _connectionModeDropdown;
        [SerializeField] private Button _connectButton;
        
        private ushort Port => ushort.Parse(_portField.text);
        private string Address => _addressField.text;

        private void OnEnable()
        {
            _connectionModeDropdown.onValueChanged.AddListener(OnConnectionModeChanged);
            _connectButton.onClick.AddListener(OnButtonConnect);
            OnConnectionModeChanged(_connectionModeDropdown.value);
        }

        private void OnDisable()
        {
            _connectionModeDropdown.onValueChanged.RemoveAllListeners();
            _connectButton.onClick.RemoveAllListeners();
        }

        private void OnConnectionModeChanged(int connectionMode)
        {
            string buttonLabel;
            _connectButton.enabled = true;
            
            switch (connectionMode)
            {
                case 0:
                    buttonLabel = "Start Client";
                    break;
                case 1 : 
                    buttonLabel = "Start Server";
                    break;
                case 2:
                    buttonLabel = "Start Host";
                    break;
                default:
                    buttonLabel = "<ERROR>";
                    _connectButton.enabled = false;
                    break;
            }

            var buttonText = _connectButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = buttonLabel;
        }

        private void OnButtonConnect()
        {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);
            
            switch (_connectionModeDropdown.value)
            {
                case 0:
                    StartClient();
                    break;
                case 1:
                    StartServer();
                    break;
                case 2:
                    StartServer();
                    StartClient();
                    break;
                default:
                    Debug.LogError("Error: Unknown connection mode", gameObject);
                    break;
            }
        }

        private static void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
        }

        private void StartServer()
        {
            var serverWorld = ClientServerBootstrap.CreateServerWorld("Game Server World");

            var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
            {
                using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
            }
        }

        private void StartClient()
        {
            var clientWorld = ClientServerBootstrap.CreateClientWorld("Game Client World");

            var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);
            {
                using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);
            }
            
            World.DefaultGameObjectInjectionWorld = clientWorld;
            var teamRequestEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(teamRequestEntity, new ClientConnectionRequest
            {
                Room = _room.text
            });
        }
    }
}