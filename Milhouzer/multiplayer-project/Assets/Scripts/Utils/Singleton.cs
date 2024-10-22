using Unity.Netcode;
using UnityEngine;

namespace Milhouzer.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    Debug.Log("Found instance of " + typeof(T) + " " + instance);
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        Debug.Log("Create instance of " + typeof(T));
                    }
                }

                return instance;
            }
        }
    }
}
