using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(Animator))]
    public class TutorialMenu : BaseMenu
    {
        [SerializeField]
        MenuCollection.MenuState nextState = MenuCollection.MenuState.Playing;

        public void OnButtonClicked()
        {
            Parent.CurrentState = nextState;
        }
    }
}
