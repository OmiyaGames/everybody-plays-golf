using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCursor : MonoBehaviour
{
    [SerializeField]
    Camera raycastCamera;
    [SerializeField]
    LayerMask raycastLayer;
    [SerializeField]
    float raycastDistance = 100f;

    public bool IsActive
    {
        get;
        set;
    }

    Ray mouseRay;
    RaycastHit mouseHit;

	// Update is called once per frame
	void Update ()
    {
        mouseRay = raycastCamera.ScreenPointToRay(Input.mousePosition);
        IsActive = Physics.Raycast(mouseRay, out mouseHit, raycastDistance, raycastLayer.value);
        if (IsActive == true)
        {
            transform.position = mouseHit.point;
        }
	}
}
