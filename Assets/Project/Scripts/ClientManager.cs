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

        [SerializeField]
        float reconnectAfter = 3f;
        [SerializeField]
        string addScoreFileName = "AddDirection.php";

        public static ClientManager Instance
        {
            get
            {
                return instance;
            }
        }

#if !SERVER
        float lastAttemptAtConnecting;
        bool isReady = false;

        void Start()
        {
            instance = this;
            RemoteSettings.Updated += RemoteSettings_Updated;
        }

        private void RemoteSettings_Updated()
        {
            isReady = true;
            Reconnect();
        }

        private void Update()
        {
            if ((isReady == true) && (IsClientConnected == false) && ((Time.time - lastAttemptAtConnecting) > reconnectAfter))
            {
                Reconnect();
            }
        }

        public void Reconnect()
        {
            Manager.StopClient();

            Debug.Log("Connecting to IP Address " + ServerIpAddress + " and Port " + Port);
            Manager.StartClient();
            lastAttemptAtConnecting = Time.time;
        }

        public void QueueDirection(Vector3 direction, string name, float netTime, System.Action<bool, string> onResult)
        {
            // Get string versions of most args
            string x = direction.x.ToString();
            string z = direction.z.ToString();
            string time = netTime.ToString();
            Debug.Log(netTime);

            // Generate MD5
            builder.Length = 0;
            builder.Append(time);
            builder.Append(x);
            builder.Append(z);
            builder.Append(name);
            builder.Append(secretKey);
            string hash = Md5Sum(builder.ToString());

            // Build header
            formData.Clear();
            formData.Add(new MultipartFormDataSection("time", time));
            formData.Add(new MultipartFormDataSection("x", x));
            formData.Add(new MultipartFormDataSection("z", z));
            formData.Add(new MultipartFormDataSection("name", name));
            formData.Add(new MultipartFormDataSection("hash", hash));
            StartCoroutine(Post(addScoreFileName, formData, onResult));
        }
#endif
    }
}
