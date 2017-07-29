using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update ()
    {
		if((Input.GetMouseButtonUp(0) == true) && (cursor.HasLocation == true))
        {
            Vector3 direction = cursor.transform.position - transform.position;
            direction.Normalize();
            direction *= maxImpulse;
            body.AddForce(direction, ForceMode.VelocityChange);
        }
	}
}
