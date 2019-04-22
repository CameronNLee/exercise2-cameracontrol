using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Obscura
{
    public class PositionLockCameraController : AbstractCameraController
    {
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

            var horizontalPointLeft = new Vector3(-2.5f, 0.0f, 85.0f);
            var horizontalPointRight = new Vector3(2.5f, 0.0f, 85.0f);

            var midPoint = new Vector3(0.0f, 0.0f, 85.0f);
            
            var verticalPointUp = new Vector3(0.0f, 2.5f, 85.0f);
            var verticalPointDown = new Vector3(0.0f, -2.5f, 85.0f);
            
            // Building horizontal line of the 5x5 cross.
            this.CameraLineRenderer.SetPosition(0, horizontalPointLeft);
            this.CameraLineRenderer.SetPosition(1, horizontalPointRight);
            
            // Make line towards the center of the player.
            this.CameraLineRenderer.SetPosition(2, midPoint);
            
            // Building vertical line of the 5x5 cross
            this.CameraLineRenderer.SetPosition(3, verticalPointUp);
            this.CameraLineRenderer.SetPosition(4, verticalPointDown);
        }
    }
}
