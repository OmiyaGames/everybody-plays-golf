using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(BoxCollider))]
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField]
        MovePlayer player;
        [SerializeField]
        float delayResetbySeconds = 0.5f;

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
            Debug.Log("Detected something...");
            if ((other.CompareTag("Player") == true) && (timeTriggered < 0f))
            {
                Debug.Log("It was the player!");
                StartCoroutine(DelayReset());
            }
        }

        IEnumerator DelayReset()
        {
            // Flag that we're resetting
            timeTriggered = Time.time;

            // Wait for a bit
            yield return WaitBeforeReset;

            // reset player position
            player.Reset();

            // Wait for a frame
            yield return null;

            // Indicate we've resetted fully
            timeTriggered = -1f;
        }
    }
}
