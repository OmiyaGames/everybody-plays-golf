using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace LudumDare39
{
    public class ServerManager : MonoBehaviour
    {
        public const int DefaultPort = 7777;
        public const string PortField = "Port";

        [SerializeField]
        float reconnectAfter = 3f;
        [SerializeField]
        SpawnPlayer spawnScript;

        float lastAttemptAtConnecting = 0f;

        public static int Port
        {
            get
            {
                return RemoteSettings.GetInt(PortField, DefaultPort);
            }
        }

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
            NetworkServer.Shutdown();

            // Attempt to reconnect again
            Debug.Log("Connecting to Port " + Port);
            NetworkServer.Listen(Port);
            lastAttemptAtConnecting = Time.time;
        }
    }
}
