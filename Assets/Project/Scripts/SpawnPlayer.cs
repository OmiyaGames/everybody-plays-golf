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

        bool isSpawned = false;

        private void Update()
        {
            if ((isSpawned == false) && (SyncPlayer.Instance == null) && (NetworkServer.active == true))
            {
                CmdSpawn();
                isSpawned = true;
            }
        }

        private void CmdSpawn()
        {
            GameObject clone = Instantiate(golfBall.gameObject, spawnLocation.position, Quaternion.identity);
            clone.GetComponent<SyncPlayer>().StartingPosition = spawnLocation.position;
            NetworkServer.Spawn(clone);
        }
    }
}
