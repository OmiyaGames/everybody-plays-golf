using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(BaseMenu))]
    public class ControlsEnabler : MonoBehaviour
    {
        static readonly HashSet<ControlsEnabler> visibleDisablers = new HashSet<ControlsEnabler>();

        static void Add(ControlsEnabler script)
        {
            if(visibleDisablers.Contains(script) == false)
            {
                visibleDisablers.Add(script);
            }
            if (MoveCursor.Instance)
            {
                MoveCursor.Instance.IsControlEnabled = false;
            }
        }

        static void Remove(ControlsEnabler script)
        {
            if (visibleDisablers.Contains(script) == true)
            {
                visibleDisablers.Remove(script);
            }
            if ((visibleDisablers.Count <= 0) && (MoveCursor.Instance))
            {
                MoveCursor.Instance.IsControlEnabled = true;
            }
        }

        private void Awake()
        {
            visibleDisablers.Clear();
        }

        void Start()
        {
            BaseMenu menu = GetComponent<BaseMenu>();
            menu.OnVisibleChanged += FocusOnHole_OnVisibleChanged;
            if (menu.IsVisible == true)
            {
                Add(this);
            }
        }

        private void FocusOnHole_OnVisibleChanged(bool arg1, bool arg2)
        {
            if (arg1 != arg2)
            {
                if (arg2)
                {
                    Add(this);
                }
                else
                {
                    Remove(this);
                }
            }
        }
    }
}
