using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;

namespace Milhouzer.Game.BuildSystem
{
    [System.Serializable]
    public class Seed
    {
        public string Target;
        public string Name;
    }


    [CreateAssetMenu(fileName = "ContextAction/Farm/PlantSeed", menuName = "ContextAction/Farm/PlantSeed", order = 0)]
    public class PlantSeed : ContextAction
    {
        public Seed Seed;

        protected override void ExecuteImpl(IContextActionHandler handler)
        {
            if(handler is not Farmland farmland) return;
            farmland.PlantSeed(Seed);
        }
    }
}