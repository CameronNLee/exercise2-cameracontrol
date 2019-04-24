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
        private PlayerController hi;
        private readonly Vector3 none = new Vector3(0,0,0);

        
        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            StartTime = Time.time;
            ElapsedTime = 0.0f;
            
            // At initialization, start and end are one and the same.
            StartPosition = this.Target.transform.position;
            EndPosition = StartPosition;
            hi = this.Target.GetComponent<PlayerController>();
            this.ManagedCamera.transform.position = new Vector3(this.Target.transform.position.x, this.Target.transform.position.y, -100.0f);
        }

        private void LateUpdate()
        {
            var huh = hi.GetMovementDirection();
            
/*            if (this.ManagedCamera.transform.position.x == this.Target.transform.position.x
                && this.ManagedCamera.transform.position.y == this.Target.transform.position.y
                && !isMoving)
            {
                ElapsedTime = 0.0f;
                StartTime = Time.time;
            }*/
            
            if (huh != none)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    StartPosition = this.ManagedCamera.transform.position;
                }
                if (ElapsedTime >= LerpDuration * 0.85f)
                {
                    // Update EndPosition to where the player Target is.
                    EndPosition = this.Target.transform.position;
                    
                    // Keep third parameter within 0 to 1 range.
                    ElapsedTime = LerpDuration * 0.85f;

                    EndPosition = this.Target.transform.position;
                    
                    var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));

                    // We want to retain default z value of -100
                    cameraPosition.z = this.ManagedCamera.transform.position.z;
                    this.ManagedCamera.transform.position = cameraPosition;
                }
                else
                {
                    // Update EndPosition to where the player Target is.
                    EndPosition = this.Target.transform.position;

                    // Linear Interpolation section.
                    // Update the camera according to Linear Interpolation scheme.
                    var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));

                    // We want to retain default z value of -100
                    cameraPosition.z = this.ManagedCamera.transform.position.z;
                    this.ManagedCamera.transform.position = cameraPosition;

                    ElapsedTime = Time.time - StartTime;
                }
            }
            else
            {
                if (this.ManagedCamera.transform.position.x != this.Target.transform.position.x
                    || this.ManagedCamera.transform.position.y != this.Target.transform.position.y)
                {
                    if (ElapsedTime >= LerpDuration)
                    {
                        ElapsedTime = LerpDuration;
                    }

                    // Player has stopped
                    if (isMoving)
                    {
                        isMoving = false;
                        StartTime = Time.time;
                        // ElapsedTime = 0.0f;
                        StartPosition = this.ManagedCamera.transform.position;
                    }

                    ElapsedTime = (Time.time - StartTime);
                    // Reset timer and update StartPosition.
                    // StartPosition = EndPosition;
                    var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));

                    // We want to retain default z value of -100
                    cameraPosition.z = this.ManagedCamera.transform.position.z;
                    this.ManagedCamera.transform.position = cameraPosition;
                }
                else
                {
                    ElapsedTime = 0.0f;
                    StartTime = Time.time;
                    StartPosition = EndPosition;
                }

            }
            
            /* if (ElapsedTime >= LerpDuration)
            {
                
            } */
            
                        
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

        private Vector3 Lerp(Vector3 start, Vector3 end, float duration)
        {
            return start + duration * (end - start);
        }
    }
}