using UnityEngine;

namespace LudumDare39
{
    public class MoveCursor : MonoBehaviour
    {
        static MoveCursor instance = null;

        [SerializeField]
        Camera raycastCamera;
        [SerializeField]
        Transform raycastPlane;

        float distance;
        Ray mouseRay;
        Plane mousePlane;

        public static MoveCursor Instance
        {
            get
            {
                return instance;
            }
        }

        public bool HasLocation
        {
            get;
            set;
        }

        private void Start()
        {
            instance = this;
            mousePlane = new Plane(Vector3.up, raycastPlane.position);
            HasLocation = false;
        }

#if !SERVER || UNITY_EDITOR
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
