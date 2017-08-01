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
            public readonly string name;

            public Direction(int id, float networkTime, Vector3 direction, string name)
            {
                this.id = id;
                this.networkTime = networkTime;
                this.direction = direction;
                this.name = name;
            }
        }

        static ServerManager instance;

        [SerializeField]
        float reconnectAfter = 3f;
        [SerializeField]
        SpawnPlayer spawnScript;
        [SerializeField]
        string getScoresFileName = "GetDirections.php";
        [SerializeField]
        string removeScoresFileName = "RemoveDirections.php";

        public static ServerManager Instance
        {
            get
            {
                return instance;
            }
        }

#if SERVER
        float lastAttemptAtConnecting;
        //bool isReady = false;

        void Start()
        {
            instance = this;
            Reconnect();
            RemoteSettings.Updated += RemoteSettings_Updated;
        }

        private void RemoteSettings_Updated()
        {
            //isReady = true;
            Reconnect();
        }

        private void Update()
        {
            //if(isReady == true)
            //{
                if((NetworkServer.active == false) && ((Time.time - lastAttemptAtConnecting) > reconnectAfter))
                {
                    Reconnect();
                }
                else if((NetworkServer.active == true) && (spawnScript.IsSpawned == false))
                {
                    spawnScript.CmdSpawn(false);
                }
            //}
        }

        void Reconnect()
        {
            // Disconnect everything
            Debug.Log("Stopping server");
            Manager.StopServer();

            // Attempt to reconnect again
            Debug.Log("Starting server on " + Manager.networkAddress + ":" + Manager.networkPort);
            Manager.StartServer();
            lastAttemptAtConnecting = Time.time;
        }

        public IEnumerator GetDirections(System.Action<bool, string> onResult)
        {
            yield return StartCoroutine(Get(getScoresFileName, onResult));
        }

        public IEnumerator RemoveDirections(IEnumerable<int> idsToRemove, System.Action<bool, string> onResult)
        {
            bool prependComma = false;

            // Generate list of IDs
            builder.Length = 0;
            foreach (int id in idsToRemove)
            {
                if (prependComma == true)
                {
                    builder.Append(',');
                }
                builder.Append(id);
                prependComma = true;
            }
            string allIds = builder.ToString();

            // Generate MD5
            builder.Append(secretKey);
            string hash = Md5Sum(builder.ToString());

            // Build header
            formData.Clear();
            formData.Add(new MultipartFormDataSection("ids", allIds));
            formData.Add(new MultipartFormDataSection("hash", hash));
            yield return StartCoroutine(Post(removeScoresFileName, formData, onResult));
        }
#endif
    }
}
