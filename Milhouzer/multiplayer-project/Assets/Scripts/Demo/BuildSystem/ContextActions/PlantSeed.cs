using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;

namespace Milhouzer.Game.BuildSystem
{
    public class Seed
    {
        public string Name;
        public char Transition;
    }


    [CreateAssetMenu(fileName = "IContextAction", menuName = "IContextAction", order = 0)]
    public class PlantSeed : ContextAction<Farmland>
    {
        public Seed Seed;

        protected override void ExecuteImpl(Farmland farmland)
        {
            farmland.PlantSeed(Seed);
        }
    }
}