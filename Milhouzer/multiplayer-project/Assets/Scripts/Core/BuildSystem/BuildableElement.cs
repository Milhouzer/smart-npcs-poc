using UnityEngine;

namespace Milhouzer.BuildingSystem
{
    /// <summary>
    /// Buildable elements are objects that can be built by the build system.
    /// Later BuildableElement may contain placement restrictions.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildableElement", menuName = "Builder/BuildableElement", order = 0)]
    public class BuildableElement : ScriptableObject {
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