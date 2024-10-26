using System;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem
{
    /// <summary>
    /// BuilderPreview handles the scene preview's lifecycle of a <see cref="BuildableElement"/>. It instantiates the model, moves, rotates and scales it.
    /// </summary>
    public class BuilderPreview
    {
        /// <summary>
        /// Element to preview
        /// </summary>
        readonly BuildableElement buildable;

        /// <summary>
        /// Current previewed object
        /// </summary>
        GameObject currentPreview;

        /// <summary>
        /// Material to use
        /// </summary>
        Material material;

        /// <summary>
        /// Collision layer of the object preview
        /// </summary>
        int layer;

        // Accessors
        public Vector3 Position => currentPreview == null ? Vector3.zero : currentPreview.transform.position;
        public Quaternion Rotation => currentPreview == null ? Quaternion.identity : currentPreview.transform.rotation;
        public Vector3 LocalScale => currentPreview == null ? Vector3.zero : currentPreview.transform.localScale;
        
        /// <summary>
        /// Default constructors, sets material, layer and buildable element. 
        /// </summary>
        /// <param name="_buildable"></param>
        /// <param name="settings"></param>
        public BuilderPreview(BuildableElement _buildable, PreviewSettings settings) {
            buildable = _buildable;
            material = settings.Material;
            layer = settings.Layer;
        }

        /// <summary>
        /// If no <see cref="currentPreview"/> exists, create one at the specified parameters, sets the transform parameters values otherwise
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        public void Preview(Vector3 pos, Quaternion rot, Vector3 scale) {
            if(currentPreview == null) {
                currentPreview = buildable.Object.Preview(pos, rot, scale);
                currentPreview.transform.localScale = scale;
                currentPreview.layer = layer;
                Renderer renderer = currentPreview.GetComponentInChildren<Renderer>();
                if(renderer) renderer.materials[0] = material;
                currentPreview.SetActive(true);
                return;
            }

            currentPreview.transform.position = pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Rotate(float amount) {
            currentPreview.transform.rotation = buildable.Constraints.Rotate(currentPreview.transform.rotation, amount);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Scale(float amount) {
            currentPreview.transform.localScale = buildable.Constraints.Scale(currentPreview.transform.localScale, amount);
        }

        public void Reset() {
            currentPreview.transform.rotation = Quaternion.identity;
            currentPreview.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Clean up the preview
        /// </summary>
        public void Cleanup() {
            if(currentPreview == null) return;

            GameObject.Destroy(currentPreview);
        }
    }
}
