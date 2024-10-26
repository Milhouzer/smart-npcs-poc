using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;

namespace Milhouzer.Game.BuildSystem
{

    [CreateAssetMenu(fileName = "ContextAction/Farm/WaterPlant", menuName = "ContextAction/Farm/WaterPlant", order = 0)]
    public class WaterPlant : ContextAction
    {
        protected override void ExecuteImpl(IContextActionHandler handler)
        {
            if(handler is not Farmland farmland) return;
            farmland.WaterPlant();
        }
    }
}