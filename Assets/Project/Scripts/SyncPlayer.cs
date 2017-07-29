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

        public NetworkIdentity Identity
        {
            get
            {
                if(identity == null)
                {
                    identity = GetComponent<NetworkIdentity>();
                }
                return identity;
            }
        }
        #endregion

        void Start()
        {
            instance = this;
        }
    }
}
