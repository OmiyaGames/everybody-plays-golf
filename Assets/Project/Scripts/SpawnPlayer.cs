using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LudumDare39
{
    public class SpawnPlayer : NetworkBehaviour
    {
        [SerializeField]
        SyncPlayer golfBall;
        [SerializeField]
        Transform spawnLocation;


        GameObject spawnedInstance = null;

        public bool IsSpawned
        {
            get
            {
                return spawnedInstance != null;
            }
        }

        public void CmdSpawn(bool alwasySpawnNewInstance)
        {
            if (NetworkServer.active == true)
            {
                // Check if there's an instance already
                if(spawnedInstance)
                {
                    if (alwasySpawnNewInstance == true)
                    {
                        // Despawn this instance
                        CmdDeSpawn();
                    }
                    else
                    {
                        // Don't spawn anything
                        return;
                    }
                }
                
                // Spawn a new instance
                spawnedInstance = Instantiate(golfBall.gameObject, spawnLocation.position, Quaternion.identity);
                spawnedInstance.GetComponent<SyncPlayer>().StartingPosition = spawnLocation.position;
                NetworkServer.Spawn(spawnedInstance);
            }
        }

        public void CmdDeSpawn()
        {
            if(spawnedInstance)
            {
                NetworkServer.Destroy(spawnedInstance);
                spawnedInstance = null;
            }
        }
    }
}
