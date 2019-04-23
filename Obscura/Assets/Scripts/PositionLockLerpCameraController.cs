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

        private void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            StartTime = Time.time;
            ElapsedTime = 0;
            
            // At initialization, start and end are one and the same.
            StartPosition = this.Target.transform.position;
            EndPosition = StartPosition;
        }

        private void LateUpdate()
        {
            if (ElapsedTime >= LerpDuration)
            {
                ElapsedTime = 0;
                // Reset timer and update StartPosition.
                StartTime = Time.time;
                StartPosition = EndPosition;

            }
            // Linear Interpolation section.
                // Update the camera according to Linear Interpolation scheme.
                var cameraPosition = Vector3.Lerp(StartPosition, EndPosition, (ElapsedTime / LerpDuration));
                
                // We want to retain default z value of -100
                cameraPosition.z = this.ManagedCamera.transform.position.z;
                this.ManagedCamera.transform.position = cameraPosition;
                

                // Update EndPosition to where the player Target is.
                EndPosition = this.Target.transform.position;
                ElapsedTime = Time.time - StartTime;
            
                        
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