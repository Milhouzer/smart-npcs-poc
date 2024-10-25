using Milhouzer.Core.BuildSystem.ContextActions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Milhouzer.UI
{
    public class ContextActionSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private ContextAction action;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(UIManager.Instance.HeldAction != null) {
                UIManager.Instance.HeldAction = null;
                return;
            }

            UIManager.Instance.HeldAction = action;
            Debug.Log($"[ContextActionSlot] Set context action {action}: {UIManager.Instance.HeldAction}");
        }
    }
}
