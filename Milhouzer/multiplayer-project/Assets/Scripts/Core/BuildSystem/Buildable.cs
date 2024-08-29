using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.BuildingSystem
{    
    /// <summary>
    /// Buildable is a component that should be present on each buildable elements of the game:
    /// - Specifies which components should be ketp when preview.
    /// - Creates the preview game object.
    /// </summary>
    public class Buildable : MonoBehaviour
    {
        /// <summary>
        /// Components to delete on the object before previewing. 
        /// </summary>
        /// <returns></returns>
        [SerializeField] List<MonoBehaviour> disableComponents = new();

        /// <summary>
        /// Create the preview of the object, deletes specified component
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        internal GameObject Preview(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            GameObject go = GameObject.Instantiate(gameObject);

            // Iterate through the components of the new instance (go), not the original prefab
            foreach (MonoBehaviour component in disableComponents)
            {
                // Find the corresponding component on the instantiated object
                var componentOnInstance = go.GetComponent(component.GetType());
                if (componentOnInstance != null)
                {
                    DestroyImmediate(componentOnInstance, true);
                }
            }

            // Disable the animator because it blocks preview scaling...
            Animator anim = GetComponentInChildren<Animator>();
            if(anim) anim.enabled = false;

            go.transform.localScale = scale;

            return go;
        }
    }
}
