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
        public enum MenuState
        {
            Connecting = -1,
            Playing = 0,
            Start,
            EnterName,
            Explanation,
            Goal,
            Controls,
            Congrats,
            EnergyCost,
            LowEnergy,
            NoEnergy,
            GameComplete
        }

        [System.Flags]
        public enum SetupState
        {
            None = 0,
            RemoteSettingsReady = 1 << 1,
            GameSettingsReady = 1 << 2,
            CursorReady = 1 << 3,
            ConnectionReady = GameSettingsReady | CursorReady,
            AllReady = RemoteSettingsReady | GameSettingsReady | CursorReady
        }

        [SerializeField]
        StartMenu startMenu;
        [SerializeField]
        BaseMenu connectingMenu;
        [SerializeField]
        BaseMenu enterName;
        [SerializeField]
        BaseMenu explanationMenu;
        [SerializeField]
        BaseMenu goalMenu;
        [SerializeField]
        BaseMenu controlsMenu;
        [SerializeField]
        BaseMenu congratsMenu;
        [SerializeField]
        BaseMenu energyCostMenu;
        [SerializeField]
        BaseMenu lowEnergyMenu;
        [SerializeField]
        BaseMenu noEnergyMenu;
        [SerializeField]
        BaseMenu gameCompleteMenu;

        [Header("Other Stuff")]
        [SerializeField]
        EnergyMeter meter;

        public static GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        static MenuCollection instance = null;
        SetupState setupState = SetupState.None;
        MenuState currentState = MenuState.Start;
        MenuState lastState = MenuState.Start;
        MenuState stateAfterEnteringName = MenuState.Playing;

#if DEBUG
        [SerializeField]
        UnityEngine.UI.Text checkState;
#endif

        readonly Dictionary<MenuState, IAnimatedMenu> stateToMenuMap = new Dictionary<MenuState, IAnimatedMenu>();

        public bool CompareState(SetupState state)
        {
            return ((setupState & state) != 0);
        }

        public static MenuCollection Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<MenuState, IAnimatedMenu> StateToMenuMap
        {
            get
            {
                if(stateToMenuMap.Count <= 0)
                {
                    // FIXME: add the rest of the menus here
                    stateToMenuMap.Add(MenuState.Start, startMenu);
                    stateToMenuMap.Add(MenuState.Connecting, connectingMenu);
                    stateToMenuMap.Add(MenuState.EnterName, enterName);
                    stateToMenuMap.Add(MenuState.Explanation, explanationMenu);
                    stateToMenuMap.Add(MenuState.Goal, goalMenu);
                    stateToMenuMap.Add(MenuState.Controls, controlsMenu);
                    stateToMenuMap.Add(MenuState.Congrats, congratsMenu);
                    stateToMenuMap.Add(MenuState.EnergyCost, energyCostMenu);
                    stateToMenuMap.Add(MenuState.LowEnergy, lowEnergyMenu);
                    stateToMenuMap.Add(MenuState.NoEnergy, noEnergyMenu);
                    stateToMenuMap.Add(MenuState.GameComplete, gameCompleteMenu);
                }
                return stateToMenuMap;
            }
        }

        public bool IsConnected
        {
            get
            {
                return ((CompareState(SetupState.ConnectionReady) == true) && (ClientManager.Instance) && (ClientManager.Instance.IsClientConnected == true));
            }
        }

        public MenuState AfterEnterName
        {
            get
            {
                return stateAfterEnteringName;
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
                    if(currentState == MenuState.GameComplete)
                    {
                        stateAfterEnteringName = currentState;
                    }
                }
            }
        }

#if !SERVER
        void Start()
        {
            // FIXME remove these
            //Settings.ClearSettings();


            setupState = SetupState.None;
            currentState = MenuState.Start;
            instance = this;
            meter.Parent = this;
            foreach (IAnimatedMenu menu in StateToMenuMap.Values)
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
            if ((CompareState(SetupState.GameSettingsReady) == false) && (SyncPlayer.Instance != null))
            {
                SetupSettings();
                setupState |= SetupState.GameSettingsReady;
            }
            if ((CompareState(SetupState.CursorReady) == false) && (MoveCursor.Instance != null))
            {
                MoveCursor.Instance.IsControlEnabled = false;
                setupState |= SetupState.CursorReady;
            }
            UpdateMenuVisibility();

#if DEBUG
            checkState.enabled = true;
            checkState.text = "setupState: " + setupState + "\nIsConnected: " + IsConnected + "\nCurrentState: " + CurrentState;
#endif
        }

        private void SetupSettings()
        {
            stateAfterEnteringName = MenuState.Playing;
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

                    // Show the game complete dialog next
                    stateAfterEnteringName = MenuState.GameComplete;
                }
            }
            Singleton.Get<GameSettings>().SaveSettings();

            // Check if we haven't seen the tutorial fully yet
            if(AddPower.HaveSeenTutorial(Settings.SeenTutorial, AddPower.TutorialFlags.ControlsTutorial) == false)
            {
                stateAfterEnteringName = MenuState.Explanation;
            }
            else if (AddPower.HaveSeenTutorial(Settings.SeenTutorial, AddPower.TutorialFlags.LowEnergy) == false)
            {
                stateAfterEnteringName = MenuState.LowEnergy;
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
