using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(Animator))]
    public class StartMenu : BaseMenu
    {
        public void OnButtonClicked()
        {
            Parent.CurrentState = MenuCollection.MenuState.EnterName;
        }
    }
}
