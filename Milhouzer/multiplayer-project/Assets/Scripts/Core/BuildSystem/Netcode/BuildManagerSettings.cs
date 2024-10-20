using UnityEngine;

namespace Milhouzer.Core.BuildSystem
{
    /// <summary>
    /// BuildManagerSettings injected in BuildManager upon it's instantiation 
    /// </summary>
    [CreateAssetMenu(fileName = "BuildManagerSettings", menuName = "Builder/BuildManagerSettings", order = 0)]
    public class BuildManagerSettings : ScriptableObject {
        /// <summary>
        /// Objects that can be built.
        /// </summary>
        public BuildableCatalog Catalog;
        /// <summary>
        /// Preview settings.
        /// </summary>
        public PreviewSettings PreviewSettings;
        /// <summary>
        /// Camera used to raycast for preview placement.
        /// </summary>
        [HideInInspector]
        public Camera Camera;

    }
}