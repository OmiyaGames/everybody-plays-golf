using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(Animator))]
    public class EnterNameMenu : BaseMenu
    {
        [SerializeField]
        TMPro.TMP_InputField field;
        [SerializeField]
        GameObject errorLabel;

        public override bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }

            set
            {
                base.IsVisible = value;
                if(value == true)
                {
                    field.text = MenuCollection.Settings.PlayerName;
                    errorLabel.SetActive(false);
                }
            }
        }

        public void OnButtonClicked()
        {
            // Check if input is value
            if (field.text.IndexOf(SyncPlayer.Divider) < 0)
            {
                // Switch scenes
                MenuCollection.Settings.PlayerName = field.text;
                errorLabel.SetActive(false);
                Parent.CurrentState = Parent.AfterEnterName;
            }
            else
            {
                // Show error
                errorLabel.SetActive(true);
            }
        }
    }
}
