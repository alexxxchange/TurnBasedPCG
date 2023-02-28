using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class Room : MonoBehaviour
    {

        public Door[] doors;
        public LootSpawnPoint[] lootSpawns;
        public EnemySpawns[] enemySpawns;
        public Transform[] nodes;

        private void OnValidate()
        {
            lootSpawns = GetComponentsInChildren<LootSpawnPoint>();
            enemySpawns = GetComponentsInChildren<EnemySpawns>();
        }

        public void SpawnDoors()
        {


            for (int i = 0; i < doors.Length; i++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(nodes[i].position, 0.5f); // check if there's something there already

                if (hitColliders.Length == 0)
                {
                    doors[i].ActivateDoor();
                }
            }


        }

    }
}
