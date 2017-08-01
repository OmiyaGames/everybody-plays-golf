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

        //void OnPlayerConnected(NetworkPlayer player)
        //{
        //    CmdSpawn(true);
        //}

        public void CmdSpawn(bool alwasySpawnNewInstance)
        {
            Vector3 spawnPosition = spawnLocation.position;
            Vector3 spawnVelocity = Vector3.zero;
            Quaternion spawnRotation = spawnLocation.rotation;
            if (NetworkServer.active == true)
            {
                // Check if there's an instance already
                if(spawnedInstance)
                {
                    if (alwasySpawnNewInstance == true)
                    {
                        // HACK: on respawn, copy the position and rotation of the original object
                        spawnPosition = spawnedInstance.transform.position;
                        spawnRotation = spawnedInstance.transform.rotation;
                        spawnVelocity = spawnedInstance.GetComponent<Rigidbody>().velocity;

                        // Despawn this instance
                        CmdDeSpawn();
                    }
                    else
                    {
                        // Don't spawn anything
                        return;
                    }
                }

                // HACK: on respawn, paste the position and rotation of the last object
                spawnedInstance = Instantiate(golfBall.gameObject, spawnPosition, spawnRotation);
                spawnedInstance.GetComponent<SyncPlayer>().StartingPosition = spawnLocation.position;
                spawnedInstance.GetComponent<Rigidbody>().velocity = spawnVelocity;

                // Spawn a new instance
                Debug.Log("Spawning golf ball");
                NetworkServer.Spawn(spawnedInstance);
            }
        }

        public void CmdDeSpawn()
        {
            if(spawnedInstance)
            {
                Debug.Log("Despawning golf ball");
                NetworkServer.Destroy(spawnedInstance);
                spawnedInstance = null;
            }
        }
    }
}
