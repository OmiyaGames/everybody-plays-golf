using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    public class ClientManager : MonoBehaviour
    {
        public const string DefaultServerUrl = "localhost";
        public const string ServerUrlField = "ServerUrl";

        public static string ServerUrl
        {
            get
            {
                return RemoteSettings.GetString(ServerUrlField, DefaultServerUrl);
            }
        }

        NetworkClient myClient;

        void Start()
        {
            myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect(ServerUrl, ServerManager.Port);
        }

        private void OnConnected(NetworkMessage netMsg)
        {
            // FIXME: Do something!
            Debug.Log("Connected to server");
        }
    }
}
