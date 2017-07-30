using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    [RequireComponent(typeof(MovePlayer))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class SyncPlayer : NetworkBehaviour
    {
        static SyncPlayer instance;

        [SyncVar]
        Vector3 startingPosition;

        MovePlayer player = null;
        NetworkIdentity identity = null;

        #region Properties
        public static SyncPlayer Instance
        {
            get
            {
                return instance;
            }
        }

        public Vector3 StartingPosition
        {
            get
            {
                return startingPosition;
            }
            set
            {
                startingPosition = value;
            }
        }

        public MovePlayer Player
        {
            get
            {
                if(player == null)
                {
                    player = GetComponent<MovePlayer>();
                }
                return player;
            }
        }
        #endregion

        public void QueueDirectionTowards(Vector3 position)
        {
            QueueDirection(position - transform.position);
        }

        public void QueueDirection(Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();

            if(ServerManager.Instance)
            {
                ServerManager.Instance.QueueDirection(direction, PrintStuff);
            }
        }

        void Start()
        {
            instance = this;
        }

        void PrintStuff(bool success, string message)
        {
            if(success)
            {
                print("success: " + message);
            }
            else
            {
                print("error: " + message);
            }
        }
    }
}
