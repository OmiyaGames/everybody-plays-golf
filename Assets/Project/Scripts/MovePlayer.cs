using UnityEngine;
using System.Collections;

namespace LudumDare39
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovePlayer : MonoBehaviour
    {
        public const string MaxImpulseField = "MaxImpulseForce";
        public const string DragField = "PlayerDrag";

        [SerializeField]
        float maxImpulse = 10f;
        [SerializeField]
        float defaultDrag = 1f;
        [SerializeField]
        Vector3 startingPosition;
        [SerializeField]
        MoveCursor cursor;
        [SerializeField]
        float checkForDragEverySeconds = 5f;

        Rigidbody body = null;
        WaitForSeconds waitEvery = null;

        #region Properties
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
        #endregion

        public void Reset()
        {
            Body.isKinematic = true;
            transform.position = startingPosition;
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
            Reset();
#if SERVER
            RemoteSettings.Updated += RemoteSettings_Updated;
            StartCoroutine(QueryRemoteSettings());
#endif
        }

#if !SERVER || UNITY_EDITOR
        void Update()
        {
            if ((Input.GetMouseButtonUp(0) == true) && (cursor.HasLocation == true))
            {
                MoveTowards(cursor.transform.position);
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

#if UNITY_EDITOR
        [ContextMenu("Set Starting Position")]
        public void SetStartingPosition()
        {
            startingPosition = transform.position;
        }
#endif
    }
}
