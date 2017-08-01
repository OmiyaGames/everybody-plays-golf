using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestServer : MonoBehaviour {

    [SerializeField]
    string ipAddress;
    [SerializeField]
    int port;

#if !UNITY_WEBGL || UNITY_EDITOR
    private void Start()
    {
        Network.Connect(ipAddress, port);
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }
#endif
}
