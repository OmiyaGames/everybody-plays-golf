using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    public class ServerManager : MonoBehaviour
    {
        public const int DefaultPort = 7777;
        public const string PortField = "Port";

        public static int Port
        {
            get
            {
                return RemoteSettings.GetInt(PortField, DefaultPort);
            }
        }

        void Start()
        {
            //NetworkServer.set
            NetworkServer.Listen(Port);
            //NetworkServer.Configure()
        }
    }
}
