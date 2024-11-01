using System.Collections.Generic;
using Milhouzer.Core;
using Milhouzer.Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Milhouzer.UI
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField]
        GameManager gameManager;
        [SerializeField]
        CanvasGroup debugConsole;

        [SerializeField] Button serverBtn;
        [SerializeField] Button hostBtn;
        [SerializeField] Button clientBtn;
        [SerializeField] Button debugConsoleBtn;

        [SerializeField] Transform players;
        [SerializeField] PlayerCard playerCard;

        Dictionary<string, PlayerCard> cards = new();

        private void Start() {
            serverBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartServer();
            });
            hostBtn.onClick.AddListener(() => {
                NetworkManager.Singleton.StartHost();
            });
            clientBtn.onClick.AddListener(() => {
                AsyncOperation op = SceneManager.LoadSceneAsync("UI", new LoadSceneParameters(LoadSceneMode.Single));
                if (op == null) throw new System.Exception("Cannot load UI scene");
                op.completed += OnUISceneLoaded;
            });
            debugConsoleBtn.onClick.AddListener(() => {
                debugConsole.Toggle();
            });
        }

        private void OnUISceneLoaded(AsyncOperation obj)
        {
            NetworkManager.Singleton.StartClient(); 
        }

        private void PlayerConnected(PlayerData playerData)
        {
            PlayerCard card = Instantiate(playerCard, players, true);

            // Pimp card
            card.SetPlayer(playerData);
            card.GetComponent<Image>().color = playerData.Color;
            card.name = playerData.Username;
            card.gameObject.SetActive(true);

            cards.Add(playerData.Username, card);
            
        }

        private void PlayerDisconnected(PlayerData data)
        {
            if (!cards.TryGetValue(data.Username, out PlayerCard card)) return;
            
            Destroy(card.gameObject);
            cards.Remove(data.Username);
        }

    }
}
