using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Obscura
{
    public class TargetFocusLerpCameraController : AbstractCameraController
    {
        [SerializeField] private float LerpDuration;
        [SerializeField] private float LeadSpeed;
        
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
            
            StartPosition = this.Target.transform.position;
            EndPosition = StartPosition;
        }

        private void LateUpdate()
        {
            if (PlayerObject.GetMovementDirection() != NoMovement)
            {
                // Moving, so use Lerp scheme
                var cameraPosition = this.ManagedCamera.transform.position;
                var targetPosition = this.Target.transform.position;
                // The conditional directly below allows us to only set
                // StartPosition once per "movement", which is what we want.
                if (!isMoving)
                {
                    isMoving = true;
                    StartPosition = this.ManagedCamera.transform.position;
                }
                
                // Update EndPosition to where the player Target is
                // PLUS the direction the player is heading towards
                // by adding the player's speed.
                var movementDirection = PlayerObject.GetMovementDirection();
                var currentSpeed = PlayerObject.GetCurrentSpeed();
                var xComponent = (currentSpeed / 10.0f) * movementDirection.x;
                var yComponent = (currentSpeed / 10.0f) * movementDirection.y;
                
                EndPosition = new Vector3(targetPosition.x + xComponent, targetPosition.y + yComponent, cameraPosition.z);

                if (ElapsedTime >= LerpDuration * 0.85f)
                {
                    // Update the camera according to threshold.
                    ElapsedTime = LerpDuration * 0.85f;
                }
                else
                {
                    ElapsedTime = (Time.time - StartTime);
                }
                
                // Update the camera according to Linear Interpolation scheme.
                cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime + LeadSpeed / LerpDuration));
                cameraPosition.z = this.ManagedCamera.transform.position.z;

                // Finally, truly update the camera position.
                this.ManagedCamera.transform.position = cameraPosition;
            }
            else
            {
                // No longer moving. Still use Lerp scheme so camera aligns with player position.
                if (!cameraHasCaughtUpToPlayer())
                {
                    if (isMoving)
                    {
                        isMoving = false;
                        StartTime = Time.time;
                        StartPosition = this.ManagedCamera.transform.position;
                    }

                    ElapsedTime = (Time.time - StartTime);
                    EndPosition = this.Target.transform.position;
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
            
            // building horizontal line of the 5x5 cross.
            this.CameraLineRenderer.SetPosition(0, horizontalPointLeft);
            this.CameraLineRenderer.SetPosition(1, horizontalPointRight);
            
            // make line towards the center of the player.
            this.CameraLineRenderer.SetPosition(2, midPoint);
            
            // building vertical line of the 5x5 cross.
            this.CameraLineRenderer.SetPosition(3, verticalPointUp);
            this.CameraLineRenderer.SetPosition(4, verticalPointDown);
        }
        
        private bool cameraHasCaughtUpToPlayer()
        {
            return (this.ManagedCamera.transform.position.x == this.Target.transform.position.x
                    && this.ManagedCamera.transform.position.y == this.Target.transform.position.y);
        }
    }
}