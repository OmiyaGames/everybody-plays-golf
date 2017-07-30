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

        NetworkManager manager = null;

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
    }
}
