﻿using System.Collections;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(BoxCollider))]
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField]
        float delayResetbySeconds = 0.5f;
        [SerializeField]
        Transform falloutThreshold;

        float timeTriggered = -1f;
        WaitForSeconds waitBeforeReset = null;

        WaitForSeconds WaitBeforeReset
        {
            get
            {
                if(waitBeforeReset == null)
                {
                    waitBeforeReset = new WaitForSeconds(delayResetbySeconds);
                }
                return waitBeforeReset;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Player") == true) && (timeTriggered < 0f))
            {
                StartCoroutine(DelayReset());
            }
        }

#if SERVER
        private void Update()
        {
            if((MovePlayer.Instance != null) && (MovePlayer.Instance.transform.position.y < falloutThreshold.position.y))
            {
                MovePlayer.Instance.Reset();
            }
        }
#endif

        IEnumerator DelayReset()
        {
            // Flag that we're resetting
            timeTriggered = Time.time;

#if SERVER
            if (MovePlayer.Instance)
            {
                // Wait for a bit
                yield return WaitBeforeReset;

                // reset player position
                MovePlayer.Instance.Reset();
            }

            // Wait for a frame
            yield return null;

            // Indicate we've resetted fully
            timeTriggered = -1f;
#else
            // Wait for a frame
            yield return null;
#endif
        }
    }
}
