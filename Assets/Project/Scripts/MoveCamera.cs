using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField]
        Transform target;
        [SerializeField]
        Vector3 offset = new Vector3(0f, 7.5f, 0f);
        [SerializeField]
        float slerpMultiplier = 5f;

        Vector3 targetPosition;

        void FixedUpdate()
        {
            targetPosition = target.position + offset;
            transform.position = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * slerpMultiplier);
        }
    }
}
