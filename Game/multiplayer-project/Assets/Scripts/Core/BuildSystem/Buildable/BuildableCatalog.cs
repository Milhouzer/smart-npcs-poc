using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Milhouzer.Core.BuildSystem
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

        public bool Get(Func<BuildableElement, bool> predicate, out BuildableElement element)
        {
            element = default;
            foreach(BuildableElement e in buildables){
                if(predicate(e))
                {
                    element = e;
                    return true;
                }
            }
            return false;
        }
    }
}