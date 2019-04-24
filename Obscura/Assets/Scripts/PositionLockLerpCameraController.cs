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
        private float StartTime;
        private float ElapsedTime;

        private Vector3 StartPosition;
        private Vector3 EndPosition;
        private bool isMoving;
        private PlayerController PlayerObject;
        private readonly Vector3 NoMovement = new Vector3(0,0,0);

        
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
                    // Keep third parameter within 0 to 1 range.
                    ElapsedTime = LerpDuration * 0.85f;                    
                }
                else
                {
                    ElapsedTime = Time.time - StartTime;
                }
                
                // Update the camera according to Linear Interpolation scheme.
                var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));
                cameraPosition.z = this.ManagedCamera.transform.position.z;
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
    }
}