using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.BuildingSystem.UI
{        
    public class BuilderUI : MonoBehaviour
    {
        [SerializeField] CatalogUI catalogUI;

        private void Awake() {
            BuildManager.OnManagerInstantiated += OnManagerInstantiated;
        }
        
        private void OnDestroy() {
            BuildManager.OnManagerInstantiated -= OnManagerInstantiated;
            BuildManager.Instance.OnEnterBuildMode -= OnEnterBuildMode;
            BuildManager.Instance.OnExitBuildMode -= OnExitBuildMode;
        }

        private void OnManagerInstantiated()
        {
            BuildManager.Instance.OnEnterBuildMode += OnEnterBuildMode;
            BuildManager.Instance.OnExitBuildMode += OnExitBuildMode;
        }

        private void OnEnterBuildMode()
        {
            catalogUI.Init(BuildManager.Instance.Catalog);
            catalogUI.TryGetComponent<CanvasGroup>(out CanvasGroup group);
            if(group != null) group.Show();
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