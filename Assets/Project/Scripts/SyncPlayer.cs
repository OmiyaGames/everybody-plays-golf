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
        [SerializeField]
        float syncEverySeconds = 0.2f;

        MovePlayer player = null;
        WaitForSeconds waitForSync = null;
        double lastNetworkTime = 0;

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

        public WaitForSeconds WaitForSync
        {
            get
            {
                if(waitForSync == null)
                {
                    waitForSync = new WaitForSeconds(syncEverySeconds);
                }
                return waitForSync;
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
            lastNetworkTime = Network.time;
            instance = this;
#if SERVER
            StartCoroutine(QueryDatabase());
#endif
        }

#if SERVER
        const char Divider = ',';
        const int IdIndex = 0;
        const int TimeIndex = IdIndex + 1;
        const int XIndex = TimeIndex + 1;
        const int ZIndex = XIndex + 1;

        static readonly char[] Newline = new char[] { '\n' };

        readonly List<ServerManager.Direction> queuedDirections = new List<ServerManager.Direction>();

        void Update()
        {
            if(queuedDirections.Count > 0)
            {
                foreach(ServerManager.Direction direction in queuedDirections)
                {
                    Player.Move(direction.direction);
                }
                queuedDirections.Clear();
            }
        }

        IEnumerator QueryDatabase()
        {
            yield return WaitForSync;

            float lastSynced, syncTime;
            while (gameObject.activeInHierarchy)
            {
                if (ServerManager.Instance)
                {
                    lastSynced = Time.time;

                    yield return ServerManager.Instance.GetDirections(lastNetworkTime, ParseDirections);

                    syncTime = (Time.time - lastSynced);
                    if(syncTime < syncEverySeconds)
                    {
                        yield return new WaitForSeconds(syncEverySeconds - syncTime);
                    }
                }
                else
                {
                    yield return WaitForSync;
                }
            }
        }

        void ParseDirections(bool status, string result)
        {
            if((status == true) && (string.IsNullOrEmpty(result) == false))
            {
                AppendDirections(result, queuedDirections);
                lastNetworkTime = GetNewestTime(queuedDirections, lastNetworkTime); 
            }
        }

        void AppendDirections(string info, ICollection<ServerManager.Direction> appendTo)
        {
            // Setup vars
            int id;
            float time, x, z;
            Vector3 direction = Vector3.zero;
            string[] cols = null;

            // split by lines
            string[] rows = info.Split(Newline, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                // Split by comma
                cols = info.Split(Divider);

                // Try to parse everything
                if ((cols.Length > ZIndex) &&
                    int.TryParse(cols[IdIndex], out id) &&
                    float.TryParse(cols[TimeIndex], out time) &&
                    float.TryParse(cols[XIndex], out x) &&
                    float.TryParse(cols[ZIndex], out z))
                {
                    // If successful, add a new direction
                    direction.x = x;
                    direction.z = z;
                    appendTo.Add(new ServerManager.Direction(id, time, direction));
                }
            }
        }

        static double GetNewestTime(ICollection<ServerManager.Direction> appendTo, double defaultTime)
        {
            double newestTime = defaultTime;
            foreach(ServerManager.Direction direction in appendTo)
            {
                if(direction.networkTime > newestTime)
                {
                    newestTime = direction.networkTime;
                }
            }
            return newestTime;
        }
#endif

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
