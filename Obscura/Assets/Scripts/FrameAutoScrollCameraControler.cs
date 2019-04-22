using UnityEngine;

namespace Obscura
{
    public class FrameAutoScrollCameraControler : AbstractCameraController
    {

        [SerializeField] public Vector2 TopLeft;
        [SerializeField] public Vector2 BottomRight;
        [SerializeField] public float AutoScrollSpeed;
        
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

            cameraPosition.x += AutoScrollSpeed;
            
            // player lagging behind the moving frame will stop at left-most box edge.
            if (targetPosition.x <= cameraPosition.x + TopLeft.x)
            {
                targetPosition.x = cameraPosition.x + TopLeft.x;
            }
            if (targetPosition.x >= cameraPosition.x + BottomRight.x)
            {
                targetPosition.x = cameraPosition.x + BottomRight.x;
            }

            if (targetPosition.y >= cameraPosition.y + TopLeft.y)
            {
                targetPosition.y = cameraPosition.y + TopLeft.y;
            }

            if (targetPosition.y <= cameraPosition.y + BottomRight.y)
            {
                targetPosition.y = cameraPosition.y + BottomRight.y;
            }

            
            // update player and camera positions
            this.Target.transform.position = targetPosition;
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
            this.CameraLineRenderer.SetPosition(0, new Vector3(TopLeft.x, TopLeft.y, 85));
            this.CameraLineRenderer.SetPosition(1, new Vector3(BottomRight.x, TopLeft.y, 85));
            this.CameraLineRenderer.SetPosition(2, new Vector3(BottomRight.x, BottomRight.y, 85));
            this.CameraLineRenderer.SetPosition(3, new Vector3(TopLeft.x, BottomRight.y, 85));
            this.CameraLineRenderer.SetPosition(4, new Vector3(TopLeft.x, TopLeft.y, 85));        
        }
    }
}