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
        #endregion

        public void Reset()
        {
            Body.isKinematic = true;
            transform.position = SyncedInfo.StartingPosition;
            Body.velocity = Vector3.zero;
            Body.isKinematic = false;
        }

        public void MoveTowards(Vector3 position)
        {
            Move(position - transform.position);
        }

        public void Move(Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();
            direction *= MaxImpulse;
            Body.AddForce(direction, ForceMode.VelocityChange);
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

#if !SERVER || UNITY_EDITOR
        void Update()
        {
            if ((MoveCursor.Instance != null) && (Input.GetMouseButtonUp(0) == true) && (MoveCursor.Instance.HasLocation == true))
            {
                MoveTowards(MoveCursor.Instance.transform.position);
            }
        }
#endif
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
