using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    public class MoveCursor : MonoBehaviour
    {
        [SerializeField]
        Camera raycastCamera;
        [SerializeField]
        LayerMask raycastLayer;
        [SerializeField]
        float raycastDistance = 100f;

        public bool HasLocation
        {
            get;
            set;
        }

        Ray mouseRay;
        RaycastHit mouseHit;

        // Update is called once per frame
        void Update()
        {
            mouseRay = raycastCamera.ScreenPointToRay(Input.mousePosition);
            HasLocation = Physics.Raycast(mouseRay, out mouseHit, raycastDistance, raycastLayer.value);
            if (HasLocation == true)
            {
                transform.position = mouseHit.point;
            }
        }
    }
}
