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
        protected const string secretKey = "0VZ;g3r0m4>Ug7a[.oi5";

        static readonly Dictionary<string, string> cachedUrls = new Dictionary<string, string>();

        protected readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
        protected readonly List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        NetworkManager manager = null;

        [SerializeField]
        protected string baseUrl = "http://omiyagames.com/epg_ld39/";

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

        #region Gets
        public IEnumerator Get(string phpFileName, System.Action<bool, string> onResult)
        {
            yield return StartCoroutine(Get(phpFileName, null, onResult));
        }

        public IEnumerator Get(string phpFileName, IDictionary<string, string> args, System.Action<bool, string> onResult)
        {
            string url = GetUrl(phpFileName);
            if((args != null) && (args.Count > 0))
            {
                bool prependAnd = false;
                builder.Length = 0;
                builder.Append(url);
                builder.Append('?');

                foreach(KeyValuePair<string, string> pair in args)
                {
                    if(prependAnd == true)
                    {
                        builder.Append('&');
                    }
                    builder.Append(pair.Key);
                    builder.Append('=');
                    builder.Append(pair.Value);
                    prependAnd = true;
                }
                url = builder.ToString();
            }

            WWW getWww = new WWW(url);
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
        #endregion

        #region Posts
        public IEnumerator Post(string phpFileName, List<IMultipartFormSection> form, System.Action<bool, string> onResult)
        {
            // Post the URL to the site and create a download object to get the result.
            UnityWebRequest postWww = UnityWebRequest.Post(GetUrl(phpFileName), form);
            yield return postWww.Send(); // Wait until the download is done

            if(onResult != null)
            {
                if (string.IsNullOrEmpty(postWww.error) == true)
                {
                    onResult(true, postWww.downloadHandler.text);
                }
                else
                {
                    onResult(false, postWww.error);
                }
            }
        }
#endregion

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
