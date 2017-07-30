﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace LudumDare39
{
    [RequireComponent(typeof(NetworkManager))]
    public class ServerManager : IManager
    {
        [SerializeField]
        float reconnectAfter = 3f;
        [SerializeField]
        SpawnPlayer spawnScript;
        [SerializeField]
        string addScoreFileName = "AddDirection.php";
        [SerializeField]
        string getScoreFileName = "GetDirections.php";

        static ServerManager instance;
        float lastAttemptAtConnecting = 0f;

        public static ServerManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void QueueDirection(Vector3 direction, System.Action<bool, string> onResult)
        {
            PostDirection(addScoreFileName, direction, onResult);
        }

        void Start()
        {
            instance = this;
            Reconnect();
        }

        private void Update()
        {
            if((NetworkServer.active == false) && ((Time.time - lastAttemptAtConnecting) > reconnectAfter))
            {
                Reconnect();
            }
            else if((NetworkServer.active == true) && (spawnScript.IsSpawned == false))
            {
                spawnScript.CmdSpawn(false);
            }
        }

        void Reconnect()
        {
            // Disconnect everything
            Manager.StopServer();

            // Attempt to reconnect again
            Debug.Log("Connecting to Port " + Port);
            Manager.StartServer();
            lastAttemptAtConnecting = Time.time;
        }
    }
}
