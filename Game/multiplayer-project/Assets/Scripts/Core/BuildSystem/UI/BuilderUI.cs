using UnityEngine;

namespace Milhouzer.Core.BuildSystem.UI
{        
    public class BuilderUI : MonoBehaviour
    {
        [SerializeField] CatalogUI catalogUI;

        public void Init()
        {
            BuildManager.Instance.OnEnterBuildMode += OnEnterBuildMode;
            BuildManager.Instance.OnExitBuildMode += OnExitBuildMode;
        }

        private void OnDestroy() {
            BuildManager.Instance.OnEnterBuildMode -= OnEnterBuildMode;
            BuildManager.Instance.OnExitBuildMode -= OnExitBuildMode;
        }

        
        private void OnEnterBuildMode()
        {
            catalogUI.Init(BuildManager.Instance.Catalog);
            catalogUI.TryGetComponent<CanvasGroup>(out CanvasGroup group);
            if(group != null) group.Show();
            
            // TODO: This should only be handled by the catalog
            BuildManager.Instance.OnBuildSelectionChange += catalogUI.OnSelectionChange;
            catalogUI.OnSelectionChange(BuildManager.Instance.Selected);
        }

        private void OnExitBuildMode()
        {
            catalogUI.TryGetComponent<CanvasGroup>(out CanvasGroup group);
            if(group != null) group.Hide();
            BuildManager.Instance.OnBuildSelectionChange -= catalogUI.OnSelectionChange;
        }

    }
}