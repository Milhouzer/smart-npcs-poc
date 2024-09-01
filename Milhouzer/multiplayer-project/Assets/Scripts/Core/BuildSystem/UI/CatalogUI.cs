using System.Linq;
using UnityEngine;

namespace Milhouzer.BuildingSystem.UI
{
    public class CatalogUI : MonoBehaviour {
        
        [SerializeField]
        Transform elementsContainer;
        /// <summary>
        /// UI element prefab
        /// </summary>
        [SerializeField]
        CatalogElement elementPrefab;

        /// <summary>
        /// Instantiated UI elements
        /// </summary>
        CatalogElement[] elements = new CatalogElement[0];

        /// <summary>
        /// Is catalog elements list initialized
        /// </summary>
        bool initialized = false;
        
        /// <summary>
        /// Init catalog elements with the provided 
        /// </summary>
        /// <param name="catalog"></param>
        public void Init(BuildableCatalog catalog) {
            if(initialized) {
                Debug.LogWarning("[CatalogUI] catalog already initialized.");
            }
            elements = new CatalogElement[catalog.Count()];
            int index = 0;
            foreach (BuildableElement buildable in catalog)
            {
                CatalogElement elem = Instantiate(elementPrefab);
                elem.transform.SetParent(elementsContainer);
                elem.SetIcon(buildable.Icon);
                elements[index] = elem;
                index++;
                elem.gameObject.SetActive(true);
            }
            initialized = true;
        }

        /// <summary>
        /// Clear instantiated UI objects
        /// </summary>
        public void Clear() {
            for (int i = elements.Length - 1; i >= 0 ; i--)
            {
                Destroy(elements[i].gameObject);
            }
            elements = null;
            initialized = false;
        }

        /// <summary>
        /// On catalog selection changed callback
        /// </summary>
        /// <param name="index"></param>
        public void OnSelectionChange(int index) {
            try
            {
                CatalogElement.Select(elements[index]);
            }
            catch (System.Exception)
            {
                Debug.LogWarning($"[CatalogUI] can't select element at {index}, elements count: {elements.Length}");
                throw;
            }
        }
    }   
}