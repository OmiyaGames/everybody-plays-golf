using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Settings;

namespace LudumDare39
{
    public class TutorialCompleteTrigger : MonoBehaviour
    {
        [SerializeField]
        AddPower.TutorialFlags completedTutorial;

        public void OnButtonClicked()
        {
            Singleton.Get<GameSettings>().SeenTutorial |= completedTutorial;
            Singleton.Get<GameSettings>().SaveSettings();
        }
    }
}
