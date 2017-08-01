using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    public class TestServerManager : IManager
    {
        void Start()
        {
            Debug.Log("Starting server on " + Manager.networkAddress + ":" + Manager.networkPort);
            Manager.StartServer();
        }
    }
}
