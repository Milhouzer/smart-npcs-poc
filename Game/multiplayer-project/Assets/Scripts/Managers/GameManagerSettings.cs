using Milhouzer.Core.BuildSystem;
using Milhouzer.UI;
using UnityEngine;
using Utils;

namespace Milhouzer.Core
{
    /// <summary>
    /// BuildManagerSettings injected in BuildManager upon it's instantiation 
    /// </summary>
    [CreateAssetMenu(fileName = "GameManagerSettings", menuName = "Game/GameManagerSettings", order = 0)]
    public class GameManagerSettings : ScriptableObject, IManagerSettings {
        
        public BuildManagerSettings BuildManagerSettings;

        public UIManagerSettings UIManagerSettings;
    }
}