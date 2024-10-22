using System;
using Milhouzer.Core.BuildSystem.ContextActions;
using Milhouzer.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Milhouzer.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public ContextAction HeldAction;

        /// <summary>
        /// Camera used to raycast from mouse when executing action.
        /// </summary>
        Camera cam;

        /// <summary>
        /// Execute the current held action on the targeted object by the mouse if not null.
        /// <see cref="HeldAction"/> becomes null after this method.
        /// </summary>
        /// <returns></returns>
        internal bool ExecuteContextAction()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("[UIManager] Cursor is over UI, canceling action.");
                HeldAction = null;
                return true;
            }

            if(HeldAction == null) return false;
            
            Debug.Log($"[UIManager] Use context action {HeldAction}");
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 150f, ~(1 << 10)))
            {
                HeldAction.Execute(hit.transform.gameObject.GetComponent<IContextActionHandler>());
                HeldAction = null;
                return true;
            }

            HeldAction = null;
            return false;
        }
    }
}
