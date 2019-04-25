using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Obscura
{
    public class FourWaySpeedupPushZoneCameraController : AbstractCameraController
    {
        [SerializeField] private float PushRatio;
        [SerializeField] private Vector2 TopLeft;
        [SerializeField] private Vector2 BottomRight;

        private Camera ManagedCamera;
        private LineRenderer CameraLineRenderer;
        private PlayerController PlayerObject;
        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            PlayerObject = this.Target.GetComponent<PlayerController>();
        }

        void LateUpdate()
        {
            var movementDirection = this.PlayerObject.GetMovementDirection();
            var playerSpeed = this.PlayerObject.GetCurrentSpeed();
            var cameraPosition = this.ManagedCamera.transform.position;

            // Note: Because playerSpeed always returns 200 or 800 (when boosting),
            // this code was tested to have worked on small PushRatio values
            // between 0.001 and 0.010. Otherwise, the x and y component variables
            // would have had values much too high when adding them to the cameraPosition,
            // assuming the camera box bounds set by the SerializedFields
            // are within an area that is 25x25 big, causing much too fast camera movement.
            var xComponent = movementDirection.x * (playerSpeed * PushRatio);
            var yComponent = movementDirection.y * (playerSpeed * PushRatio);

            xComponent += cameraPosition.x;
            yComponent += cameraPosition.y;
            
            // Update camera position according to PushRatio scheme.
            cameraPosition = new Vector3(xComponent, yComponent, cameraPosition.z);
            
            // Update camera position according to PushBoxCamera, if applicable.
            cameraPosition = PushBoxCameraModifier(cameraPosition);
            
            // Finally, update the actual in-game camera.
            this.ManagedCamera.transform.position = cameraPosition;
            
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
            this.CameraLineRenderer.SetPosition(0, new Vector3(TopLeft.x, TopLeft.y, 85.0f));
            this.CameraLineRenderer.SetPosition(1, new Vector3(BottomRight.x, TopLeft.y, 85.0f));
            this.CameraLineRenderer.SetPosition(2, new Vector3(BottomRight.x, BottomRight.y, 85.0f));
            this.CameraLineRenderer.SetPosition(3, new Vector3(TopLeft.x, BottomRight.y, 85.0f));
            this.CameraLineRenderer.SetPosition(4, new Vector3(TopLeft.x, TopLeft.y, 85.0f));
        }

        // Sets the camera speed to the player speed if player touches
        // the outer push zone border box. If the player isn't touching
        // any border box, then the function parameter is returned
        // with no modifications as is.
        private Vector3 PushBoxCameraModifier(Vector3 cameraPosition)
        {
            var targetPosition = this.Target.transform.position;
            if (targetPosition.y >= cameraPosition.y + TopLeft.y)
            {
                cameraPosition = new Vector3(cameraPosition.x, targetPosition.y - TopLeft.y, cameraPosition.z);
            }
            if (targetPosition.y <= cameraPosition.y + BottomRight.y)
            {
                cameraPosition = new Vector3(cameraPosition.x, targetPosition.y- BottomRight.y, cameraPosition.z);
            }
            if (targetPosition.x >= cameraPosition.x + BottomRight.x)
            {
                cameraPosition = new Vector3(targetPosition.x - BottomRight.x, cameraPosition.y, cameraPosition.z);
            }
            if (targetPosition.x <= cameraPosition.x + TopLeft.x)
            {
                cameraPosition = new Vector3(targetPosition.x- TopLeft.x, cameraPosition.y, cameraPosition.z);
            }

            return cameraPosition;
        }
        
    }
}