using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Milhouzer.CameraSystem
{
    public interface ICameraController {
        void LookAt(Transform target);
        void Zoom();
        void Dezoom();
    }

    public class CameraController : NetworkBehaviour, ICameraController
    {
        [Header("Camera")]
        [SerializeField]
        Camera cam;

        [SerializeField]
        Transform _target;

        [Header("Controller")]
        [SerializeField]
        Vector3 trackedObjectOffset;
        
        [SerializeField]
        float switchTargetSpeed = 0;
        [SerializeField]
        float followTargetSpeed = 0;

        [SerializeField]
        float lookAtThreshold = 0.05f;
        [SerializeField]
        float rotationSpeed = 0f;

        [SerializeField]
        Vector2 zoomLimits = new Vector2(0.3f, 1.5f);
        [SerializeField]
        float zoomSpeed = 0f;

        bool _hasReachedNewTarget = false;
        float currentZoomLevel = 1f;
        Vector3 currentObjectOffset;
        Coroutine zoomRoutine;
        
        Transform _lastTarget;

        public float Priority { 
            get { return cam.depth; }
            set { cam.depth = value; }
        }
        void OnValidate() {
            currentObjectOffset = transform.position - _target.position;
            transform.position = _target.position + currentObjectOffset;
            
            Vector3 targetDirection = _target.position - currentObjectOffset;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation;
            Debug.Log(transform.position);
        }


        void Update() 
        {
            if(!IsOwner) return;

            Aim();
        }

        public void LookAt(Transform target)
        {
            _lastTarget = _target;
            _target = target;
            _hasReachedNewTarget = target == null ? true : false;
        }

        public void ResetZoom()
        {
            ZoomTowards(1f);
        }

        public void Zoom()
        {
            ZoomTowards(zoomLimits.x);
        }

        public void Dezoom()
        {
            ZoomTowards(zoomLimits.y);
        }

        public void ZoomTowards(float value)
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
            }
            zoomRoutine = StartCoroutine(Zoom(value, zoomSpeed));
        }

        void Aim()
        {
            Vector3 finalPos = _target.position + currentObjectOffset;
            transform.position = Vector3.Lerp(transform.position, finalPos, (_hasReachedNewTarget ? followTargetSpeed : switchTargetSpeed) * Time.deltaTime);
            
            float d = Vector3.Distance(transform.position, finalPos);
            if(d < lookAtThreshold)
            {
                SmoothLookAtTarget();
                float dinit = (_lastTarget == null || _hasReachedNewTarget )? 0 : Vector3.Distance(_lastTarget.transform.position, _target.transform.position);
                if((dinit - d) > dinit * 0.9f)
                    _hasReachedNewTarget = true;
            }
        }

        void SmoothLookAtTarget()
        {
            Vector3 targetDirection = _target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        IEnumerator Zoom(float value, float zoomSpeed)
        {
            while(Mathf.Abs(currentZoomLevel - value) > 0.01f)
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, value, zoomSpeed);
                currentObjectOffset = trackedObjectOffset * currentZoomLevel;
                yield return null;
            }
            Mathf.Lerp(currentZoomLevel, value, zoomSpeed);
            currentZoomLevel = value;
            currentObjectOffset = trackedObjectOffset * currentZoomLevel;
            zoomRoutine = null;
        }

        internal Ray ScreenToPointRay()
        {
            Vector3 mousePosition = UnityEngine.Input.mousePosition;
            return cam.ScreenPointToRay(mousePosition);
        }
    }
}
