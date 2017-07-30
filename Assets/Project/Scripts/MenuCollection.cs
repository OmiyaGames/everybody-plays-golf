using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OmiyaGames;
using OmiyaGames.Settings;

namespace LudumDare39
{
    public class MenuCollection : MonoBehaviour
    {
#if !SERVER
        public enum MenuState
        {
            Connecting = -1,
            Playing = 0,
            Start,

        }

        [System.Flags]
        public enum SetupState
        {
            None = 0,
            RemoteSettingsReady = 1 << 1,
            GameSettingsReady = 1 << 2,
            AllReady = RemoteSettingsReady | GameSettingsReady
        }

        [SerializeField]
        StartMenu startMenu;

        SetupState setupState = SetupState.None;
        MenuState currentState = MenuState.Start;
        MenuState lastState = MenuState.Start;

        readonly Dictionary<MenuState, IAnimatedMenu> stateToMenuMap = new Dictionary<MenuState, IAnimatedMenu>();

        public Dictionary<MenuState, IAnimatedMenu> StateToMenuMap
        {
            get
            {
                if(stateToMenuMap.Count <= 0)
                {
                    // FIXME: add the rest of the menus here
                    stateToMenuMap.Add(MenuState.Start, startMenu);
                }
                return stateToMenuMap;
            }
        }

        public bool IsConnected
        {
            get
            {
                return ((setupState == SetupState.AllReady) && (ClientManager.Instance) && (ClientManager.Instance.Manager.client.isConnected == true));
            }
        }

        public MenuState CurrentState
        {
            get
            {
                if((IsConnected == false) && (currentState != MenuState.Start))
                {
                    return MenuState.Connecting;
                }
                else
                {
                    return currentState;
                }
            }
            set
            {
                if(currentState != value)
                {
                    lastState = currentState;
                    currentState = value;
                    if (StateToMenuMap.ContainsKey(lastState) == true)
                    {
                        StateToMenuMap[lastState].IsVisible = false;
                    }
                }
            }
        }

        public static GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        void Start()
        {
            setupState = SetupState.None;
            currentState = MenuState.Start;
            foreach(IAnimatedMenu menu in StateToMenuMap.Values)
            {
                menu.Parent = this;
            }
            RemoteSettings.Updated += RemoteSettings_Updated;
        }

        private void RemoteSettings_Updated()
        {
            setupState |= SetupState.RemoteSettingsReady;
        }

        private void Update()
        {
            if ((setupState == SetupState.RemoteSettingsReady) && (SyncPlayer.Instance != null))
            {
                SetupSettings();
                setupState |= SetupState.GameSettingsReady;
            }
            UpdateMenuVisibility();
        }

        private static void SetupSettings()
        {
            if (Settings.LastGameID == AddPower.DefaultGameId)
            {
                // This is the first time the player started.
                // Update the stats as if it's the first time playing.
                Settings.LastGameID = SyncPlayer.Instance.GameId;
                Settings.CurrentEnergy = AddPower.MaxEnergy;
                Settings.LastMaxEnergy = AddPower.MaxEnergy;
            }
            else
            {
                // First, check if the max energy field are different
                if (Settings.LastMaxEnergy < AddPower.MaxEnergy)
                {
                    // If the max energy increased since we last played, increase the current energy as well
                    Settings.CurrentEnergy = (AddPower.MaxEnergy - Settings.LastMaxEnergy);
                }

                // Check if the current energy doesn't exceed the max energy
                if (Settings.CurrentEnergy > AddPower.MaxEnergy)
                {
                    // If it does, cap it
                    Settings.CurrentEnergy = AddPower.MaxEnergy;
                }

                // Update max energy
                Settings.LastMaxEnergy = AddPower.MaxEnergy;

                // Check if while the player was away, a new game has started
                if (Settings.LastGameID != SyncPlayer.Instance.GameId)
                {
                    // If so, max out the energy
                    Settings.CurrentEnergy = AddPower.MaxEnergy;

                    // Update the game ID
                    Settings.LastGameID = SyncPlayer.Instance.GameId;
                }
            }
        }

        private void UpdateMenuVisibility()
        {
            IAnimatedMenu menu;
            if (lastState != CurrentState)
            {
                if (StateToMenuMap.TryGetValue(lastState, out menu) == true)
                {
                    menu.IsVisible = false;
                }
                if (StateToMenuMap.TryGetValue(CurrentState, out menu) == true)
                {
                    menu.IsVisible = true;
                }
                lastState = CurrentState;
            }
        }
#endif
    }
}
