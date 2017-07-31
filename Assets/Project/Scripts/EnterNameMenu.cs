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
        [SerializeField]
        TMPro.TMP_Text label;

        [SerializeField]
        string invalidCharMsg = "Invalid char";
        [SerializeField]
        string cannotBeEmptyMsg = "Name cannot be empty";

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
            if(string.IsNullOrEmpty(field.text) == true)
            {
                // Show error
                label.text = cannotBeEmptyMsg;
                errorLabel.SetActive(true);
            }
            else if (field.text.IndexOf(SyncPlayer.Divider) >= 0)
            {
                // Show error
                label.text = invalidCharMsg;
                errorLabel.SetActive(true);
            }
            else
            {
                // Switch scenes
                MenuCollection.Settings.PlayerName = field.text;
                errorLabel.SetActive(false);
                Parent.CurrentState = Parent.AfterEnterName;
            }
        }
    }
}
