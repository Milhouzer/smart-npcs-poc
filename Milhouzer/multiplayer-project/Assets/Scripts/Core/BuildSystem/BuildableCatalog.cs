using UnityEngine;

namespace Milhouzer.BuildingSystem
{
    [CreateAssetMenu(fileName = "BuildableCatalog", menuName = "Builder/BuildableCatalog", order = 0)]
    public class BuildableCatalog : ScriptableObject {
        [SerializeField]
        CircularBuffer<BuildableElement> buildables;

        public BuildableElement this[int index] => buildables.Get(index);
    }
}