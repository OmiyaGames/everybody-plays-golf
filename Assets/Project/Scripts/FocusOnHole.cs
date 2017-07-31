using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(BaseMenu))]
    public class FocusOnHole : MonoBehaviour
    {
        [SerializeField]
        float zoomLevel = 1;

        void Start()
        {
            GetComponent<BaseMenu>().OnVisibleChanged += FocusOnHole_OnVisibleChanged;
        }

        private void FocusOnHole_OnVisibleChanged(bool arg1, bool arg2)
        {
            if(arg1 != arg2)
            {
                if(arg2)
                {
                    // Focus on the hole
                    MoveCamera.Instance.Target = GoalTrigger.Instance.transform;
                    MoveCamera.Instance.Multiplier = zoomLevel;
                }
                else
                {
                    // Unfocus from the hold
                    MoveCamera.Instance.FocusOnPlayer();
                    MoveCamera.Instance.Multiplier = MoveCamera.DefaultOverrideZoomMultiplier;
                }
            }
        }
    }
}
