// using Unity.Netcode;
// using UnityEngine;

// namespace Milhouzer.Utils
// {
//     public class NetworkedSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
//     {
//         private static T instance;

//         public static T Instance
//         {
//             get
//             {
//                 if (instance == null)
//                 {
//                     instance = FindObjectOfType<T>();
//                     Debug.Log("Found instance of " + typeof(T) + " " + instance);
//                     if (instance == null)
//                     {
//                         GameObject singletonObject = new GameObject();
//                         instance = singletonObject.AddComponent<T>();
//                         singletonObject.name = typeof(T).ToString() + " (Singleton)";
//                         Debug.Log("Create instance of " + typeof(T));
//                     }
//                 }

//                 return instance;
//             }
//         }

//         public override void OnNetworkSpawn()
//         {
//             base.OnNetworkSpawn();
            
//             if (instance == null)
//             {
//                 instance = this as T;
//             }
//             else
//             {
//                 Debug.Log("[" + typeof(T).ToString() + "] Already exists, deleting this object");
//                 Destroy(gameObject);
//             }
//         }
//     }
// }
