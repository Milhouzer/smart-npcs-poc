using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem
{    
    /// <summary>
    /// 
    /// </summary>
    public interface IStateObject
    {
        /// <summary>
        /// Id of the stateholder
        /// </summary>
        /// <value></value>
        // public int Index { get; set; }

        /// <summary>
        /// State identifier
        /// </summary>
        /// <value></value>
        public string UID { get; set; }

        /// <summary>
        /// Destroy the state holder object.
        /// </summary>
        public void Destroy();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBuildable
    {
        /// <summary>
        /// Create the preview of the object, deletes specified component
        /// </summary>
        /// <param name="pos">position of the preview object</param>
        /// <param name="rot">rotation of the preview object</param>
        /// <param name="scale">scale of the preview object</param>
        /// <returns>The preview object</returns>
        public GameObject Preview(Vector3 pos, Quaternion rot, Vector3 scale);

        /// <summary>
        /// Build the object in the world
        /// </summary>
        /// <param name="pos">position of the build object</param>
        /// <param name="rot">rotation of the build object</param>
        /// <param name="scale">scale of the build object</param>
        /// <returns>The built object</returns>
        public GameObject Build(Vector3 pos, Quaternion rot, Vector3 scale);
    }

    /// <summary>
    /// Buildable is a component that should be present on each buildable elements of the game:
    /// - Specifies which components should be ketp when preview.
    /// - Creates the preview game object.
    /// </summary>
    public class Buildable : MonoBehaviour, IBuildable
    {
        /// <summary>
        /// Components to delete on the object before previewing. 
        /// </summary>
        /// <returns></returns>
        [SerializeField] List<Component> disableComponents = new();
        
        public GameObject Preview(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            GameObject go = GameObject.Instantiate(gameObject);

            foreach (Component component in disableComponents)
            {
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

        public GameObject Build(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            GameObject go = GameObject.Instantiate(gameObject);
            GameObject.Destroy(go.GetComponent<Buildable>());
            
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.localScale = scale;
            go.SetActive(true);

            return go;
        }
    }
}
