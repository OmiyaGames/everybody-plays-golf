using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    public class SpawnPlayer : NetworkBehaviour
    {
        [SerializeField]
        float delaySpawning = 1f;
        [SerializeField]
        SyncPlayer golfBall;
        [SerializeField]
        Transform spawnLocation;

        // Use this for initialization
        IEnumerator Start()
        {
            yield return new WaitForSeconds(delaySpawning);
            while(SyncPlayer.Instance == null)
            {
                yield return null;
                if (NetworkServer.active == true)
                {
                    CmdSpawn();
                    break;
                }
            }
        }

        private void Update()
        {
            if ((NetworkServer.active == true) && (Input.GetKeyUp(KeyCode.Space) == true))
            {
                CmdSpawn();
            }
        }

        //[Command]
        private void CmdSpawn()
        {
            GameObject clone = Instantiate(golfBall.gameObject, spawnLocation.position, Quaternion.identity);
            clone.GetComponent<SyncPlayer>().StartingPosition = spawnLocation.position;
            NetworkServer.Spawn(clone);
        }
    }
}
