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
        string addScoreFileName = "AddDirection.php";

        float time;

        public static ClientManager Instance
        {
            get
            {
                return instance;
            }
        }

#if !SERVER
        void Start()
        {
            instance = this;
            time = Time.time;
            Manager.StartHost();
        }

        public void QueueDirection(Vector3 direction, string name, System.Action<bool, string> onResult)
        {
            // Get string versions of most args
            string x = direction.x.ToString();
            string z = direction.z.ToString();
            string time = Network.time.ToString();

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

#if UNITY_EDITOR && RECONNECT
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
#endif
    }
}
