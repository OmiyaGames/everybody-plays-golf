using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    [RequireComponent(typeof(NetworkManager))]
    public abstract class IManager : MonoBehaviour
    {
        public const int DefaultPort = 7777;
        public const string DefaultServerIpAddress = "127.0.0.1";
        public const string PortField = "Port";
        public const string ServerIpAddressField = "ServerIpAddress";
        const string secretKey = "0VZ;g3r0m4>Ug7a[.oi5";

        NetworkManager manager = null;
        static readonly Dictionary<string, string> cachedUrls = new Dictionary<string, string>();
        readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
        readonly List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        [SerializeField]
        protected string baseUrl = "http://omiyagames.com/epg_ld39/";
        [SerializeField]
        protected string getScoreFileName = "GetDirections.php";
        [SerializeField]
        protected string addScoreFileName = "AddDirection.php";
        [SerializeField]
        protected string removeScoreFileName = "RemoveDirection.php";

        public static string ServerIpAddress
        {
            get
            {
                return RemoteSettings.GetString(ServerIpAddressField, DefaultServerIpAddress);
            }
        }

        public static int Port
        {
            get
            {
                return RemoteSettings.GetInt(PortField, DefaultPort);
            }
        }

        public NetworkManager Manager
        {
            get
            {
                if(manager == null)
                {
                    manager = GetComponent<NetworkManager>();
                    manager.networkAddress = ServerIpAddress;
                    manager.networkPort = Port;
                }
                return manager;
            }
        }

        private string GetUrl(string phpFileName)
        {
            string fullUrl;
            if(cachedUrls.TryGetValue(phpFileName, out fullUrl) == false)
            {
                fullUrl = baseUrl + phpFileName;
                cachedUrls.Add(phpFileName, fullUrl);
            }
            return fullUrl;
        }

        public IEnumerator Get(string phpFileName, System.Action<bool, string> onResult)
        {
            WWW getWww = new WWW(GetUrl(phpFileName));
            yield return getWww;

            if(onResult != null)
            {
                if (string.IsNullOrEmpty(getWww.error) == true)
                {
                    onResult(true, getWww.text);
                }
                else
                {
                    onResult(false, getWww.error);
                }
            }
        }

        public IEnumerator Post(string phpFileName, List<IMultipartFormSection> form, System.Action<bool, string> onResult)
        {
            // Post the URL to the site and create a download object to get the result.
            UnityWebRequest postWww = UnityWebRequest.Post(GetUrl(phpFileName), form);
            yield return postWww.Send(); // Wait until the download is done

            if(onResult != null)
            {
                if (string.IsNullOrEmpty(postWww.error) == true)
                {
                    onResult(true, null);
                }
                else
                {
                    onResult(false, postWww.error);
                }
            }
        }

        public IEnumerator PostDirection(string phpFileName, Vector3 direction, System.Action<bool, string> onResult)
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
            builder.Append(secretKey);
            string hash = Md5Sum(builder.ToString());

            // Build header information
            builder.Length = 0;
            builder.Append("time=");
            builder.Append(time);
            builder.Append("&x=");
            builder.Append(x);
            builder.Append("&z=");
            builder.Append(z);
            builder.Append("&hash=");
            builder.Append(hash);

            // Build header
            formData.Clear();
            print(builder.ToString());
            //formData.Add(new MultipartFormDataSection(builder.ToString()));
            formData.Add(new MultipartFormDataSection("time", time));
            formData.Add(new MultipartFormDataSection("x", x));
            formData.Add(new MultipartFormDataSection("z", z));
            formData.Add(new MultipartFormDataSection("hash", hash));
            yield return StartCoroutine(Post(phpFileName, formData, onResult));
        }

        public void QueueDirection(Vector3 direction, System.Action<bool, string> onResult)
        {
            StartCoroutine(PostDirection(addScoreFileName, direction, onResult));
        }

        /// <summary>
        /// Taken from http://wiki.unity3d.com/index.php?title=MD5
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        public static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }
    }
}
