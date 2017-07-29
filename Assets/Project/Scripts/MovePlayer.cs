using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovePlayer : MonoBehaviour
    {
        [SerializeField]
        float maxImpulse = 10f;
        [SerializeField]
        Vector3 startingPosition;
        [SerializeField]
        MoveCursor cursor;

        Rigidbody body;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        public void Reset()
        {
            Debug.Log("Resetting player position");
            body.isKinematic = true;
            transform.position = startingPosition;
            body.velocity = Vector3.zero;
            body.isKinematic = false;
        }

        public void MoveTowards(Vector3 position)
        {
            Move(position - transform.position);
        }

        public void Move(Vector3 direction)
        {
            direction.Normalize();
            direction *= maxImpulse;
            body.AddForce(direction, ForceMode.VelocityChange);
        }

        void Update()
        {
            if ((Input.GetMouseButtonUp(0) == true) && (cursor.HasLocation == true))
            {
                MoveTowards(cursor.transform.position);
            }
        }


#if UNITY_EDITOR
        [ContextMenu("Set Starting Position")]
        public void SetStartingPosition()
        {
            startingPosition = transform.position;
        }
#endif
    }
}
