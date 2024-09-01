using System.Collections.Generic;
using UnityEngine;

namespace Milhouzer.BuildingSystem
{
    [CreateAssetMenu(fileName = "BuildableCatalog", menuName = "Builder/BuildableCatalog", order = 0)]
    public class BuildableCatalog : ScriptableObject, IEnumerable<BuildableElement> {
        [SerializeField]
        BuildableElement[] buildables;

        public BuildableElement this[int index] => buildables[index];
        
        // Implement the GetEnumerator method for IEnumerable<BuildableElement>
        public IEnumerator<BuildableElement> GetEnumerator()
        {
            for (int i = 0; i < buildables.Length; i++)
            {
                yield return buildables[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}