using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;

namespace Milhouzer.Game.BuildSystem
{

    [CreateAssetMenu(fileName = "ContextAction/Farm/HarvestPlant", menuName = "ContextAction/Farm/HarvestPlant", order = 0)]
    public class HarvestPlant : ContextAction
    {
        protected override void ExecuteImpl(IContextActionHandler handler)
        {
            if(handler is not Farmland farmland) return;
            farmland.HarvestPlant();
        }
    }
}