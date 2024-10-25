using UnityEngine;

namespace Milhouzer.UI
{
    /// <summary>
    /// UIManagerSettings injected in UIManager upon it's instantiation 
    /// </summary>
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "UI/UIManagerSettings", order = 0)]
    public class UIManagerSettings : ScriptableObject {
        
        public Camera Camera;
    }
}