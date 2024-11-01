using UnityEngine;

namespace Milhouzer.Core.BuildSystem
{
    /// <summary>
    /// Buildable elements are objects that can be built by the build system.
    /// Later BuildableElement may contain placement restrictions.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildableElement", menuName = "Builder/BuildableElement", order = 0)]
    public class BuildableElement : ScriptableObject {
        /// <summary>
        /// Buildable element identifier
        /// This ID will be used to identify the element when built. Ids should be unique, by convention they follow the syntax :
        /// *.*.* and so on... (i.e. FARMLAND.TOMATO.SEED, FARMLAND.TOMATO.SMALL, FARMLAND.TOMATO.LARGE)
        /// </summary>
        public string UID;
        
        /// <summary>
        /// Object to build
        /// </summary>
        public Buildable Object;

        /// <summary>
        /// Constraints of the preview (rotation, scaling, etc.)
        /// </summary>
        public PreviewConstraints Constraints;

        /// <summary>
        /// Icon of the preview object displayed in the UI.
        /// </summary>
        /// <remarks>
        /// This shouldn't be here right ?
        /// </remarks>
        public Sprite Icon;
    }
}