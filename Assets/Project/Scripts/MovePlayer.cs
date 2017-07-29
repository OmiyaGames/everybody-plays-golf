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
        MoveCursor cursor;

        Rigidbody body;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
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
    }
}
