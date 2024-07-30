using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _icon;
    [SerializeField] Image _infos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _infos.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _infos.gameObject.SetActive(false);
    }

    // [SerializeField] PlayerInfos Infos;

    public void SetPlayer(PlayerData player) {
        _icon.color = player.Color;
        _infos.color = player.Color;
    }
}
