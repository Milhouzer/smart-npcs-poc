using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField] Button serverBtn;
    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;

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
            NetworkManager.Singleton.StartClient();
        });

        gameManager.OnPlayerConnectedCallback += PlayerConnected;
        gameManager.OnPlayerDisconnectedCallback += PlayerDisconnected;
    }
    
    private void PlayerConnected(PlayerData playerData)
    {
        Debug.Log("okok");
        PlayerCard card = Instantiate(playerCard);

        // Pimp card
        card.SetPlayer(playerData);

        card.transform.SetParent(players);
        card.GetComponent<Image>().color = playerData.Color;
        card.name = playerData.Username;
        card.gameObject.SetActive(true);

        cards.Add(playerData.Username, card);
    }

    private void PlayerDisconnected(PlayerData data)
    {
        bool ok = cards.TryGetValue(data.Username, out PlayerCard card);
        if(ok) {
            Destroy(card.gameObject);
            cards.Remove(data.Username);
        }
    }

}