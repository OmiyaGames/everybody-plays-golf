using UnityEngine;

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

        Rigidbody body = null;

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
            direction.Normalize();
            direction *= MaxImpulse;
            Body.AddForce(direction, ForceMode.VelocityChange);
        }

        #region Unity Events
        private void FixedUpdate()
        {
            Body.drag = Drag;
        }

#if !SERVER
        void Update()
        {
            if ((Input.GetMouseButtonUp(0) == true) && (cursor.HasLocation == true))
            {
                MoveTowards(cursor.transform.position);
            }
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
