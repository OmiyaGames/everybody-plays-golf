using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OmiyaGames;
using OmiyaGames.Settings;

namespace LudumDare39
{
    [RequireComponent(typeof(MovePlayer))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class SyncPlayer : NetworkBehaviour
    {
        static SyncPlayer instance;

        [SerializeField]
        float syncEverySeconds = 0.2f;

        [SyncVar]
        Vector3 startingPosition;
        [SyncVar]
        float syncTime;
        [SyncVar]
        int gameId;

        MovePlayer player = null;
        WaitForSeconds waitForSync = null;

        #region Properties
        public static SyncPlayer Instance
        {
            get
            {
                return instance;
            }
        }

        public static GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
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

        public float NetworkTime
        {
            get
            {
                return syncTime;
            }
        }

        public int GameId
        {
            get
            {
                return gameId;
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

        void Start()
        {
            instance = this;
#if SERVER
            gameId = 0;
            StartCoroutine(QueryDatabase());
#endif
        }

        public void SetupNextGame()
        {
            gameId += 1;
            Settings.LastGameID = gameId;
            Settings.CurrentEnergy = Settings.LastMaxEnergy;
        }

#if SERVER
        const char Divider = '|';
        const int IdIndex = 0;
        const int TimeIndex = IdIndex + 1;
        const int XIndex = TimeIndex + 1;
        const int ZIndex = XIndex + 1;
        const int NameIndex = ZIndex + 1;

        static readonly char[] Newline = new char[] { '\n' };

        readonly List<ServerManager.Direction> queuedDirections = new List<ServerManager.Direction>();
        readonly HashSet<int> readIds = new HashSet<int>();

        void Update()
        {
            syncTime = (float)Network.time;
            if(queuedDirections.Count > 0)
            {
                foreach(ServerManager.Direction direction in queuedDirections)
                {
                    if(readIds.Contains(direction.id) == false)
                    {
                        Player.Move(direction.direction);
                        readIds.Add(direction.id);
                    }
                }

                queuedDirections.Clear();
                if((readIds.Count > 0) && (ServerManager.Instance))
                {
                    StartCoroutine(ServerManager.Instance.RemoveDirections(readIds, RemoveIds));
                }
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

                    yield return ServerManager.Instance.GetDirections(ParseDirections);
                    yield return ServerManager.Instance.GetDirections(ParseDirections);

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
                // Go through each line, and split by comma
                cols = row.Split(Divider);

                // Try to parse everything
                if ((cols.Length > NameIndex) &&
                    int.TryParse(cols[IdIndex], out id) &&
                    float.TryParse(cols[TimeIndex], out time) &&
                    float.TryParse(cols[XIndex], out x) &&
                    float.TryParse(cols[ZIndex], out z))
                {
                    // If successful, add a new direction
                    direction.x = x;
                    direction.z = z;
                    appendTo.Add(new ServerManager.Direction(id, time, direction, cols[NameIndex]));
                }
            }
        }

        void RemoveIds(bool status, string result)
        {
            if((status == true) && (string.IsNullOrEmpty(result) == false))
            {
                int removeId;
                string[] ids = result.Split(',');
                foreach(string id in ids)
                {
                    if((int.TryParse(id, out removeId) == true) && (readIds.Contains(removeId) == true))
                    {
                        readIds.Remove(removeId);
                    }
                }
            }
        }
#else
        public void QueueDirection(Vector3 direction)
        {
            // Queue the direction into this manager
            if (ClientManager.Instance != null)
            {
                ClientManager.Instance.QueueDirection(direction, Singleton.Get<GameSettings>().PlayerName, syncTime, PrintStuff);
            }
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
