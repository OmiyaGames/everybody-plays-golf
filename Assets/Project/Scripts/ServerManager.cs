using UnityEngine;
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

        float lastAttemptAtConnecting = 0f;

        void Start()
        {
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
