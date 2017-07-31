using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(Animator))]
    public class MoveCursor : MonoBehaviour
    {
        public const string VisibleField = "Visible";
        static MoveCursor instance = null;

        [SerializeField]
        Camera raycastCamera;
        [SerializeField]
        Transform raycastPlane;

        float distance;
        Ray mouseRay;
        Plane mousePlane;
        Animator animator;

        bool controlEnabled = true;
        bool hasLocation = false;

        public static MoveCursor Instance
        {
            get
            {
                return instance;
            }
        }

        public bool HasLocation
        {
            get
            {
                if(IsControlEnabled == true)
                {
                    return hasLocation;
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                hasLocation = value;
            }
        }

        public bool IsControlEnabled
        {
            get
            {
                return controlEnabled;
            }
            set
            {
                controlEnabled = value;
                Animator.SetBool(VisibleField, controlEnabled);
            }
        }

        public Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        private void Start()
        {
            instance = this;
            mousePlane = new Plane(Vector3.up, raycastPlane.position);
            HasLocation = false;
        }

        void Update()
        {
            HasLocation = false;
            if (IsControlEnabled == true)
            {
                mouseRay = raycastCamera.ScreenPointToRay(Input.mousePosition);
                HasLocation = mousePlane.Raycast(mouseRay, out distance);
                if (HasLocation == true)
                {
                    transform.position = mouseRay.GetPoint(distance);
                }
            }
        }
    }
}
