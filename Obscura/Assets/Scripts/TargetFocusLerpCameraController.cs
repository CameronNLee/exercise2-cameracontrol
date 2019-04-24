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
        
        public void Awake()
        {
            this.ManagedCamera = this.gameObject.GetComponent<Camera>();
            this.CameraLineRenderer = this.gameObject.GetComponent<LineRenderer>();
            PlayerObject = this.Target.GetComponent<PlayerController>();
            
            StartTime = Time.time;
            ElapsedTime = 0.0f;
            
            StartPosition = this.ManagedCamera.transform.position;
            EndPosition = StartPosition;
        }

        private void LateUpdate()
        {
            if (PlayerObject.GetMovementDirection() != NoMovement)
            {
                // Moving, so use Lerp scheme
                var cameraPosition = this.ManagedCamera.transform.position;
            }
            else
            {
                // No longer moving. Still use Lerp scheme so camera aligns with player position.
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
    }
}