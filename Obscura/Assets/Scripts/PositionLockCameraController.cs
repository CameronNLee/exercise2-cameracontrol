using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class PositionLockCameraController : AbstractCameraController
    {
        [SerializeField] public Vector3 MiddleLeft;
        [SerializeField] public Vector3 MiddleRight;
        [SerializeField] public Vector3 MiddleUp;
        [SerializeField] public Vector3 MiddleDown;
        private Camera ManagedCamera;
        private LineRenderer CameraLineRenderer;

        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
        }

        // Use the LateUpdate message to avoid setting the camera's position before
        // GameObject locations are finalized.
        void LateUpdate()
        {
            var targetPosition = this.Target.transform.position;
            var cameraPosition = this.ManagedCamera.transform.position;
            this.ManagedCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, cameraPosition.z);

            if (this.DrawLogic)
            {
                this.CameraLineRenderer.enabled = true;
                this.DrawCameraLogic();
            }
            else
            {
                this.CameraLineRenderer.enabled = false;
            }
        }

        public override void DrawCameraLogic()
        {
            this.CameraLineRenderer.positionCount = 5;
            this.CameraLineRenderer.useWorldSpace = false;
            
            // building horizontal line of the 5x5 cross
            this.CameraLineRenderer.SetPosition(0, MiddleLeft);
            this.CameraLineRenderer.SetPosition(1, MiddleRight);
            
            // make line towards the center of the player
            this.CameraLineRenderer.SetPosition(2, new Vector3(0, 0, 85));
            
            // building vertical line of the 5x5 cross
            this.CameraLineRenderer.SetPosition(3, MiddleUp);
            this.CameraLineRenderer.SetPosition(4, MiddleDown);
        }
    }
}
