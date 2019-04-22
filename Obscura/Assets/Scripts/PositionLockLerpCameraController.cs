using UnityEngine;

namespace Obscura
{
    public class PositionLockLerpCameraController : AbstractCameraController
    {
        [SerializeField] private float LerpDuration;
        
        private Camera ManagedCamera;
        private LineRenderer CameraLineRenderer;

        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
        }

        private void LateUpdate()
        {
            var targetPosition = this.Target.transform.position;
            var cameraPosition = this.ManagedCamera.transform.position;
        }

        public override void DrawCameraLogic()
        {
            // fill in
        }
    }
}