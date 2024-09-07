using System;
// using Unity.Netcode;
// using Unity.Netcode.Components;
using UnityEngine;

public class DefaultEntity /*: NetworkBehaviour, ISelectable*/
{
    // [SerializeField] NetworkAnimator animator;

    // private NetworkVariable<ulong> _isSelected = new NetworkVariable<ulong>(k_DefaultSelectedValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // private const ulong k_DefaultSelectedValue = ulong.MaxValue;

    // // Any player can override entity selection for now, may change later.
    // public bool IsSelected() => false /* _isSelected.Value != ulong.MaxValue */;

    // public override void OnNetworkSpawn()
    // {
    //     if(!IsServer) 
    //     {
    //         _isSelected.OnValueChanged += OnSelectionChanged;
    //         if(_isSelected.Value != k_DefaultSelectedValue) {
    //             OnSelectionChanged(k_DefaultSelectedValue, _isSelected.Value);
    //         }
    //         Debug.Log($"{NetworkManager.Singleton.LocalClientId}: {_isSelected.Value}");
    //     }
    // }

    // private void OnSelectionChanged(ulong previousValue, ulong newValue)
    // {
    //     Debug.Log($"Value has changed from {previousValue} to {newValue}");

    //     if(newValue == ulong.MaxValue) 
    //     {
    //         // entityRenderer.material.SetColor("_FirstOutlineColor", Color.white);
    //         // entityRenderer.material.SetColor("_SecondOutlineColor", Color.white);
    //         return;
    //     }

    //     if(GameManager.Instance.TryGetPlayerData(newValue, out PlayerData data))
    //     {
    //         // entityRenderer.material.SetColor("_FirstOutlineColor", data.Color);
    //         // entityRenderer.material.SetColor("_SecondOutlineColor", data.Color);
    //     }
    // }

    // public void Select(ulong selector)
    // {
    //     if(!IsServer) return;

    //     animator.SetTrigger("LayEgg");
    //     _isSelected.Value = selector;

    //     // Ultimately, server does not need any graphics update.
    //     if(GameManager.Instance.TryGetPlayerData(selector, out PlayerData data))
    //     {
    //         // entityRenderer.material.SetColor("_FirstOutlineColor", data.Color);
    //         // entityRenderer.material.SetColor("_SecondOutlineColor", data.Color);
    //     }
    // }

    // public void Unselect()
    // {
    //     if(!IsServer) return;

    //     // Ultimately, server does not need any graphics update.
    //     // entityRenderer.material.SetColor("_FirstOutlineColor", Color.white);
    //     // entityRenderer.material.SetColor("_SecondOutlineColor", Color.white);
    //     _isSelected.Value = ulong.MaxValue;
    // }

    // public GameObject Preview()
    // {
    //     throw new NotImplementedException();
    // }

    // public void Build()
    // {
    //     throw new NotImplementedException();
    // }
}