using UnityEngine;

namespace LudumDare39
{
    public class MoveCursor : MonoBehaviour
    {
        [SerializeField]
        Camera raycastCamera;
        [SerializeField]
        Transform raycastPlane;

        public bool HasLocation
        {
            get;
            set;
        }

        float distance;
        Ray mouseRay;
        Plane mousePlane;

        private void Start()
        {
            mousePlane = new Plane(Vector3.up, raycastPlane.position);
            HasLocation = false;
        }

#if !SERVER
        void Update()
        {
            mouseRay = raycastCamera.ScreenPointToRay(Input.mousePosition);
            HasLocation = mousePlane.Raycast(mouseRay, out distance);
            if (HasLocation == true)
            {
                transform.position = mouseRay.GetPoint(distance);
            }
        }
#endif
    }
}
