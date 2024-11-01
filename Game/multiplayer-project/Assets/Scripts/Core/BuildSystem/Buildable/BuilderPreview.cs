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
        private readonly BuildableElement _buildable;

        /// <summary>
        /// Current previewed object
        /// </summary>
        private GameObject _currentPreview;

        /// <summary>
        /// Material to use
        /// </summary>
        private readonly Material _material;

        /// <summary>
        /// Collision layer of the object preview
        /// </summary>
        private readonly int _layer;

        // Accessors
        public Vector3 Position => _currentPreview == null ? Vector3.zero : _currentPreview.transform.position;
        public Quaternion Rotation => _currentPreview == null ? Quaternion.identity : _currentPreview.transform.rotation;
        public Vector3 LocalScale => _currentPreview == null ? Vector3.zero : _currentPreview.transform.localScale;
        
        /// <summary>
        /// Default constructors, sets material, layer and buildable element. 
        /// </summary>
        /// <param name="buildable"></param>
        /// <param name="settings"></param>
        public BuilderPreview(BuildableElement buildable, PreviewSettings settings) {
            this._buildable = buildable;
            _material = settings.Material;
            _layer = settings.Layer;
        }

        /// <summary>
        /// If no <see cref="_currentPreview"/> exists, create one at the specified parameters, sets the transform parameters values otherwise
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        public void Preview(Vector3 pos, Quaternion rot, Vector3 scale) {
            if(!_currentPreview) {
                _currentPreview = _buildable.Object.Preview(pos, rot, scale);
                _currentPreview.transform.localScale = scale;
                _currentPreview.layer = _layer;
                Renderer renderer = _currentPreview.GetComponentInChildren<Renderer>();
                if(renderer) renderer.materials[0] = _material;
                _currentPreview.SetActive(true);
                return;
            }

            _currentPreview.transform.position = pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Rotate(float amount) {
            _currentPreview.transform.rotation = _buildable.Constraints.Rotate(_currentPreview.transform.rotation, amount);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Scale(float amount) {
            _currentPreview.transform.localScale = _buildable.Constraints.Scale(_currentPreview.transform.localScale, amount);
        }

        public void Reset() {
            _currentPreview.transform.rotation = Quaternion.identity;
            _currentPreview.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Clean up the preview
        /// </summary>
        public void Cleanup() {
            if(!_currentPreview) return;

            Object.Destroy(_currentPreview);
        }
    }
}
