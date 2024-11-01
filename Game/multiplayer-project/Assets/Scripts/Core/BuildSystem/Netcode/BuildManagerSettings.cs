using Milhouzer.Core.BuildSystem.StatesManagement;
using UnityEngine;
using Utils;

namespace Milhouzer.Core.BuildSystem
{
    /// <summary>
    /// BuildManagerSettings injected in BuildManager upon it's instantiation 
    /// </summary>
    [CreateAssetMenu(fileName = "BuildManagerSettings", menuName = "Builder/BuildManagerSettings", order = 0)]
    public class BuildManagerSettings : ScriptableObject, IManagerSettings {
        /// <summary>
        /// Objects that can be built.
        /// </summary>
        public BuildableCatalog Catalog;
        /// <summary>
        /// Preview settings.
        /// </summary>
        public PreviewSettings PreviewSettings;

        /// <summary>
        /// StatesManager settings
        /// </summary>
        public StatesNamespace Namespace;
    }
}