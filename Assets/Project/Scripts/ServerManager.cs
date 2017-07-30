using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace LudumDare39
{
    [RequireComponent(typeof(NetworkManager))]
    public class ServerManager : IManager
    {
        public struct Direction
        {
            public readonly int id;
            public readonly float networkTime;
            public readonly Vector3 direction;

            public Direction(int id, float networkTime, Vector3 direction)
            {
                this.id = id;
                this.networkTime = networkTime;
                this.direction = direction;
            }
        }

        static ServerManager instance;

        [SerializeField]
        float reconnectAfter = 3f;
        [SerializeField]
        SpawnPlayer spawnScript;
        [SerializeField]
        string getScoresFileName = "GetDirections.php";

        float lastAttemptAtConnecting = 0f;
        readonly Dictionary<string, string> cachedArgs = new Dictionary<string, string>();

        public static ServerManager Instance
        {
            get
            {
                return instance;
            }
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

        public IEnumerator GetDirections(double networkTime, System.Action<bool, string> onResult)
        {
            if (cachedArgs.ContainsKey("time") == true)
            {
                cachedArgs["time"] = networkTime.ToString();
            }
            else
            {
                cachedArgs.Add("time", networkTime.ToString());
            }
            yield return StartCoroutine(Get(getScoresFileName, cachedArgs, onResult));
        }
    }
}
