// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using Unity.Netcode;
// using UnityEngine.UIElements;

// public class NetworkManagerHelper : MonoBehaviour
// {
//     public static NetworkManagerHelper Instance;

//     private SimpleEnableDisable m_SimpleEnableDisable;

//     public void RegisterComponent(SimpleEnableDisable simpleEnableDisable)
//     {
//         m_SimpleEnableDisable = simpleEnableDisable;
//     }
//     private void Start()
//     {
//         Screen.SetResolution((int)(Screen.currentResolution.width * 0.5f), (int)(Screen.currentResolution.height * 0.5f), FullScreenMode.Windowed);
//     }

//     private void OnGUI()
//     {
//         var networkManager = NetworkManager.Singleton;
//         if (!networkManager.IsClient && !networkManager.IsServer)
//         {
//             GUILayout.BeginArea(new Rect(10, 10, 300, 800));
//             if (GUILayout.Button("Host"))
//             {
//                 networkManager.StartHost();
//             }

//             if (GUILayout.Button("Client"))
//             {
//                 networkManager.StartClient();
//             }
//             GUILayout.EndArea();

//             GUILayout.BeginArea(new Rect(10, Display.main.renderingHeight - 40, Display.main.renderingWidth - 10, 30));
//             var scenesPreloaded = new System.Text.StringBuilder();
//             scenesPreloaded.Append("Scenes Preloaded: ");
//             for (int i = 0; i < SceneManager.sceneCount; i++)
//             {
//                 var scene = SceneManager.GetSceneAt(i);
//                 scenesPreloaded.Append($"[{scene.name}]");
//             }
//             GUILayout.Label(scenesPreloaded.ToString());
//             GUILayout.EndArea();
//         }
//         else
//         {
//             GUILayout.BeginArea(new Rect(10, 10, 300, 800));
//             GUILayout.Label($"Mode: {(networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client")}");
//             if (networkManager.IsServer && m_SimpleEnableDisable != null)
//             {
//                 GUILayout.BeginHorizontal();
//                 var updateVisuals = false;
//                 foreach (var entry in m_SimpleEnableDisable.Visuals)
//                 {
//                     var enabled = (m_SimpleEnableDisable.VisualStates.Value & entry.VisualFlags) == entry.VisualFlags;
//                     var changedState = GUILayout.Toggle(enabled, entry.VisualFlags.ToString());
//                     if (changedState != enabled)
//                     {
//                         m_SimpleEnableDisable.SetVisualActive(entry.VisualFlags, changedState, false);
//                         updateVisuals = true;
//                     }
//                 }
//                 if (updateVisuals)
//                 {
//                     m_SimpleEnableDisable.UpdateVisuals();
//                 }
//                 GUILayout.EndHorizontal();
//             }

//             if (m_MessageLogs.Count > 0)
//             {
//                 GUILayout.Label("-----------(Log)-----------");
//                 // Display any messages logged to screen
//                 foreach (var messageLog in m_MessageLogs)
//                 {
//                     GUILayout.Label(messageLog.Message);
//                 }
//                 GUILayout.Label("---------------------------");
//             }
//             GUILayout.EndArea();

//             GUILayout.BeginArea(new Rect(Display.main.renderingWidth - 40, 10, 30, 30));

//             if (GUILayout.Button("X"))
//             {
//                 networkManager.Shutdown();
//             }
//             GUILayout.EndArea();
//         }

//     }

//     private void Update()
//     {
//         if (m_MessageLogs.Count == 0)
//         {
//             return;
//         }

//         for (int i = m_MessageLogs.Count - 1; i >= 0; i--)
//         {
//             if (m_MessageLogs[i].ExpirationTime < Time.realtimeSinceStartup)
//             {
//                 m_MessageLogs.RemoveAt(i);
//             }
//         }
//     }

//     private List<MessageLog> m_MessageLogs = new List<MessageLog>();

//     private class MessageLog
//     {
//         public string Message { get; private set; }
//         public float ExpirationTime { get; private set; }

//         public MessageLog(string msg, float timeToLive)
//         {
//             Message = msg;
//             ExpirationTime = Time.realtimeSinceStartup + timeToLive;
//         }
//     }

//     public void LogMessage(string msg, float timeToLive = 10.0f)
//     {
//         if (m_MessageLogs.Count > 0)
//         {
//             m_MessageLogs.Insert(0, new MessageLog(msg, timeToLive));
//         }
//         else
//         {
//             m_MessageLogs.Add(new MessageLog(msg, timeToLive));
//         }

//         Debug.Log(msg);
//     }

//     public NetworkManagerHelper()
//     {
//         Instance = this;
//     }
// }
