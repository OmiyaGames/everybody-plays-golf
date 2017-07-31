using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace LudumDare39
{
    [RequireComponent(typeof(SyncPlayer))]
    [RequireComponent(typeof(Rigidbody))]
    public class MovePlayer : MonoBehaviour
    {
        public const string MaxImpulseField = "MaxImpulseForce";
        public const string DragField = "PlayerDrag";

        static MovePlayer instance = null;

        [SerializeField]
        float maxImpulse = 10f;
        [SerializeField]
        float defaultDrag = 1f;
        [SerializeField]
        float checkForDragEverySeconds = 5f;

        SyncPlayer syncInfo = null;
        Rigidbody body = null;
        WaitForSeconds waitEvery = null;

        #region Properties
        public static MovePlayer Instance
        {
            get
            {
                return instance;
            }
        }

        Rigidbody Body
        {
            get
            {
                if(body == null)
                {
                    body = GetComponent<Rigidbody>();
                }
                return body;
            }
        }

        public float MaxImpulse
        {
            get
            {
                return RemoteSettings.GetFloat(MaxImpulseField, maxImpulse);
            }
        }

        public float Drag
        {
            get
            {
                return RemoteSettings.GetFloat(DragField, defaultDrag);
            }
        }

        public WaitForSeconds WaitEvery
        {
            get
            {
                if(waitEvery == null)
                {
                    waitEvery = new WaitForSeconds(checkForDragEverySeconds);
                }
                return waitEvery;
            }
        }

        public SyncPlayer SyncedInfo
        {
            get
            {
                if(syncInfo == null)
                {
                    syncInfo = GetComponent<SyncPlayer>();
                }
                return syncInfo;
            }
        }

        public bool CanMove
        {
            get
            {
#if SERVER || UNITY_EDITOR
                return ((MoveCursor.Instance != null) && (MoveCursor.Instance.HasLocation == true));
#else
                return ((MoveCursor.Instance != null) && (MoveCursor.Instance.HasLocation == true));
#endif
            }
        }
        #endregion

#if SERVER
        public void Reset(bool isNewGame)
        {
            // Move the Rigidbody
            Body.isKinematic = true;
            transform.position = SyncedInfo.StartingPosition;
            Body.velocity = Vector3.zero;
            Body.isKinematic = false;
            
            // Setup the next game
            if(isNewGame == true)
            {
                SyncedInfo.SetupNextGame();
            }
        }
#endif

        public void Move(Vector3 direction)
        {
            Move(direction, true);
        }

        private void Move(Vector3 direction, bool doNormalize)
        {
            if (doNormalize == true)
            {
                NormalizeDirection(ref direction);
            }
            direction *= MaxImpulse;
            Body.AddForce(direction, ForceMode.VelocityChange);
        }

        private void NormalizeDirection(ref Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();
        }

        #region Unity Events
        private void Start()
        {
            // Setup instance
            instance = this;

#if SERVER
            RemoteSettings.Updated += RemoteSettings_Updated;
            StartCoroutine(QueryRemoteSettings());
#endif

            // Focus the camera on the player
            if (MoveCamera.Instance)
            {
                MoveCamera.Instance.FocusOnPlayer();
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) == true)
            {
                if (CanMove == true)
                {
                    // Calculate the direction to move
                    Vector3 direction = MoveCursor.Instance.transform.position - transform.position;
                    NormalizeDirection(ref direction);

#if SERVER
                    // If the server, just move the ball directly
                    Move(direction, false);
#else
                    // If the client, send to the database the direction you've entered
                    SyncedInfo.QueueDirection(direction);
#endif
                }
                else
                {
                    // FIXME: notify player can't move
                }
            }
        }
#endregion

#region Helper Methods
#if SERVER
        IEnumerator QueryRemoteSettings()
        {
            while(gameObject.activeInHierarchy == true)
            {
                yield return WaitEvery;
                RemoteSettings.ForceUpdate();
            }
        }

        private void RemoteSettings_Updated()
        {
            Body.drag = Drag;
        }
#endif
#endregion
    }
}
