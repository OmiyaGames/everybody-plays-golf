using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    public class MenuCollection : MonoBehaviour
    {
#if !SERVER
        public enum State
        {
            Connecting = -1,
            Playing = 0,
            Start,

        }

        [SerializeField]
        StartMenu startMenu;

        bool isReady = false;
        State currentState = State.Start;
        State lastState = State.Start;

        readonly Dictionary<State, IAnimatedMenu> stateToMenuMap = new Dictionary<State, IAnimatedMenu>();

        public Dictionary<State, IAnimatedMenu> StateToMenuMap
        {
            get
            {
                if(stateToMenuMap.Count <= 0)
                {
                    // FIXME: add the rest of the menus here
                    stateToMenuMap.Add(State.Start, startMenu);
                }
                return stateToMenuMap;
            }
        }

        public bool IsConnected
        {
            get
            {
                return ((isReady == true) && (ClientManager.Instance) && (ClientManager.Instance.Manager.client.isConnected == true));
            }
        }

        public State CurrentState
        {
            get
            {
                if((IsConnected == false) && (currentState != State.Start))
                {
                    return State.Connecting;
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

        void Start()
        {
            isReady = false;
            currentState = State.Start;
            foreach(IAnimatedMenu menu in StateToMenuMap.Values)
            {
                menu.Parent = this;
            }
            RemoteSettings.Updated += RemoteSettings_Updated;
        }

        private void RemoteSettings_Updated()
        {
            isReady = true;
        }

        private void Update()
        {
            IAnimatedMenu menu;
            if(lastState != CurrentState)
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
