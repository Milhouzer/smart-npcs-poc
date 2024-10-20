using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;
using Milhouzer.Core.BuildSystem;

namespace Milhouzer.Game.BuildSystem
{
    public class Farmland : MonoBehaviour, IStateObject, IContextActionHandler
    {
        [HideInInspector]
        public char Identifier { get; set; }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void PlantSeed(Seed seed){

            BuildManager.Instance.RequestBuildUpdate(this, Identifier, seed.Transition);
        }
    }
}
