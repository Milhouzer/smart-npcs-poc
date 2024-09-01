using UnityEngine;
using UnityEngine.UI;

namespace Milhouzer.BuildingSystem.UI
{
    public class CatalogElement : MonoBehaviour {
        [SerializeField] Image Icon;
        [SerializeField] GameObject SelectionOutline;
        
        public void SetIcon(Sprite icon) {
            Icon.sprite = icon;
        }
        
        public void Select() {
            SelectionOutline.SetActive(true);
        }

        public void Unselect() {
            SelectionOutline.SetActive(false);
        }

        /// <summary>
        /// Current catalog selected item.
        /// </summary>
        private static CatalogElement CurrentSelected;

        /// <summary>
        /// Select a new element
        /// </summary>
        public static void Select(CatalogElement elem) {
            if(CatalogElement.CurrentSelected != null) {
                CatalogElement.CurrentSelected.Unselect();
            }

            CatalogElement.CurrentSelected = elem;
            if(elem != null) elem.Select();
        }
    }
}