using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Obscura
{
    public class PositionLockLerpCameraController : AbstractCameraController
    {
        [SerializeField] private float LerpDuration;
        
        private Camera ManagedCamera;
        private LineRenderer CameraLineRenderer;
        private PlayerController PlayerObject;

        private float StartTime;
        private float ElapsedTime;

        private Vector3 StartPosition;
        private Vector3 EndPosition;
        private readonly Vector3 NoMovement = new Vector3(0,0,0);

        private bool isMoving;

        
        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            PlayerObject = this.Target.GetComponent<PlayerController>();
            StartTime = Time.time;
            ElapsedTime = 0.0f;
            
            // At initialization, start and end are one and the same.
            StartPosition = this.Target.transform.position;
            EndPosition = StartPosition;
        }

        private void LateUpdate()
        {
            if (PlayerObject.GetMovementDirection() != NoMovement)
            {
                var cameraPosition = this.ManagedCamera.transform.position;
                // The conditional directly below allows us to only set
                // StartPosition once per "movement", which is what we want.
                if (!isMoving)
                {
                    isMoving = true;
                    StartPosition = this.ManagedCamera.transform.position;
                }
                
                // Update EndPosition to where the player Target is.
                EndPosition = this.Target.transform.position;
                
                // Camera is set to not-quite catch up to the moving player
                // past a certain threshold (85% of LerpDuration).
                if (ElapsedTime >= LerpDuration * 0.85f)
                {
                    // Update the camera according to PushBoxCamera scheme.
                    cameraPosition = getPushBoxCameraPosition();
                }
                else
                {
                    ElapsedTime = (Time.time - StartTime);
                    // Update the camera according to Linear Interpolation scheme.
                    cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));
                    cameraPosition.z = this.ManagedCamera.transform.position.z;
                }
                
                // Finally, truly update the camera position.
                this.ManagedCamera.transform.position = cameraPosition;
            }
            else
            {
                if (!cameraHasCaughtUpToPlayer())
                {
                    // Once again, we use this to set the StartPosition only once
                    // per "non-movement". Likewise, reset the StartTime here ONCE.
                    if (isMoving)
                    {
                        isMoving = false;
                        StartTime = Time.time;
                        StartPosition = this.ManagedCamera.transform.position;
                    }

                    ElapsedTime = (Time.time - StartTime);
                    // Keep updating the camera according to Linear interpolation
                    // until the camera catches up to the player who is no longer moving.
                    var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));
                    cameraPosition.z = this.ManagedCamera.transform.position.z;
                    this.ManagedCamera.transform.position = cameraPosition;
                }
                else
                {
                    // Camera position is now equal to player position, so reset these values.
                    ElapsedTime = 0.0f;
                    StartTime = Time.time;
                    StartPosition = EndPosition;
                }
            }
                        
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
            
            // building horizontal line of the 5x5 cross
            this.CameraLineRenderer.SetPosition(0, horizontalPointLeft);
            this.CameraLineRenderer.SetPosition(1, horizontalPointRight);
            
            // make line towards the center of the player
            this.CameraLineRenderer.SetPosition(2, midPoint);
            
            // building vertical line of the 5x5 cross
            this.CameraLineRenderer.SetPosition(3, verticalPointUp);
            this.CameraLineRenderer.SetPosition(4, verticalPointDown);
        }

        // Helper function that checks if camera position is the player's position.
        // If so, that means that the camera has caught up to the player.
        private bool cameraHasCaughtUpToPlayer()
        {
            return (this.ManagedCamera.transform.position.x == this.Target.transform.position.x
                    && this.ManagedCamera.transform.position.y == this.Target.transform.position.y);
        }

        // Helper function code borrowed from PushBoxCamera.cs script.
        // Used to prevent player from going too far out of camera bounds when moving.
        private Vector3 getPushBoxCameraPosition()
        {
            var targetPosition = this.Target.transform.position;
            var cameraPosition = this.ManagedCamera.transform.position;
            
            // Set how big to draw the pushbox boundaries based on some constant factor
            // times the LerpDuration. I.e. the bigger the LerpDuration, the bigger the boundaries.
            var boundingFactor = this.LerpDuration * 20.0f;
            
            // Restrict the pushbox from becoming too big
            // if too high of a LerpDuration is entered.
            if (boundingFactor >= 45.0f)
            {
                boundingFactor = 45.0f;
            }
            if (targetPosition.y >= cameraPosition.y + boundingFactor)
            {
                cameraPosition = new Vector3(cameraPosition.x, targetPosition.y - boundingFactor, cameraPosition.z);
            }

            if (targetPosition.y <= cameraPosition.y - boundingFactor)
            {
                cameraPosition = new Vector3(cameraPosition.x, targetPosition.y + boundingFactor, cameraPosition.z);
            }

            if (targetPosition.x >= cameraPosition.x + boundingFactor)
            {
                cameraPosition = new Vector3(targetPosition.x - boundingFactor, cameraPosition.y, cameraPosition.z);
            }

            if (targetPosition.x <= cameraPosition.x - boundingFactor)
            {
                cameraPosition = new Vector3(targetPosition.x + boundingFactor, cameraPosition.y, cameraPosition.z);
            }

            return cameraPosition;
        }
    }
}