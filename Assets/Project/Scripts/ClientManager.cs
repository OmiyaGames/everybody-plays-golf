using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    [RequireComponent(typeof(NetworkManager))]
    public class ClientManager : IManager
    {
        static ClientManager instance;
        float time;

        public static ClientManager Instance
        {
            get
            {
                return instance;
            }
        }

        void Start()
        {
            instance = this;
            time = Time.time;
            Manager.StartHost();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if ((Time.time - time) > 3)
            {
                time = Time.time;
                Manager.StopClient();
                Manager.StartHost();
            }
        }
#endif
    }
}
