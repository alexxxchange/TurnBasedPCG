using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System.Linq;

namespace LevelGeneration
{

    public class LevelGenerator : MonoBehaviour
    {

        [Header("options")]
        [SerializeField] bool spawnDoors;
        [SerializeField] bool spawnEnemies;
        [SerializeField] bool spawnLoot;
        [SerializeField] int targetNumberOfRooms = 12;

        [Header("level Generation")]
        [SerializeField] Transform levelParent;
        [SerializeField] GameObject startRoom;
        [SerializeField] GameObject[] levelRooms;
        [SerializeField] GameObject endRoomPrefab;
        [Header("loot")]
        [SerializeField] GameObject[] lootPrefabs;


        [Header("enemies")]
        [SerializeField] GameObject[] enemyPrefabs;

        [Header("these lists fill during generation. visible for debugging only")]
        [SerializeField] List<Door> allDoors;
        [SerializeField] List<Room> roomsInLevel;
        [SerializeField] List<Transform> roomSpawnNodes;
        [SerializeField] List<LootSpawnPoint> allLootSpawns;
        [SerializeField] List<EnemySpawns> allEnemySpawns;

        float pcgDelayTime = 0.1f;
        Room endRoomCached;
        int generationLayer = 0; //number of times we've iterated


        private void Start()
        {
            StartCoroutine(GenerateLevel());
        }

        IEnumerator GenerateLevel()
        {
            GenerateStartRoom();
            yield return new WaitForEndOfFrame();
            while (roomsInLevel.Count < targetNumberOfRooms)
            {
                GetNodes();
                yield return new WaitForEndOfFrame();
                GenerateRooms();
                yield return new WaitForSeconds(pcgDelayTime);
            }
            Debug.Log("roomSpawnComplete");
            yield return new WaitForSeconds(pcgDelayTime);

            GetNodes();
            yield return new WaitForEndOfFrame();
            GenerateEndRoom();
            yield return new WaitForSeconds(pcgDelayTime);

            if (spawnDoors)
            {
                GenerateDoors();
                yield return new WaitForSeconds(pcgDelayTime);
            }
            if (spawnEnemies)
            {
                GenerateEnemies();
                yield return new WaitForSeconds(pcgDelayTime);
            }

            if (spawnLoot)
                GenerateLoot();
        }

        private void GenerateEndRoom()
        {
            Transform lastRoomSpawn = roomSpawnNodes.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).Last();
            GameObject temp = Instantiate(endRoomPrefab, lastRoomSpawn.position, Quaternion.identity);
            temp.transform.SetParent(levelParent);
            Room thisRoom = temp.GetComponent<Room>();
            //roomsInLevel.Add(thisRoom); // maybe maybe not?... in case we want to do anything with the filler rooms at runtime like lock all doors etc...

            endRoomCached = thisRoom;
        }

        public void GenerateStartRoom()
        {
            GameObject temp = Instantiate(startRoom, transform.position, Quaternion.identity);
            temp.transform.SetParent(levelParent);
            Room thisRoom = temp.GetComponent<Room>();
            roomsInLevel.Add(thisRoom);
        }

        private void GetNodes()
        {
            roomSpawnNodes.Clear();// clear list
            List<Transform> tempRoomSpawnNodes = new List<Transform>(); // create temp list

            for (int i = 0; i < roomsInLevel.Count; i++) // add all rooms to temp list
            {
                Room tempRoom = roomsInLevel[i]; 
                for (int j = 0; j < tempRoom.nodes.Length; j++)
                {
                    tempRoomSpawnNodes.Add(tempRoom.nodes[j]); // add all nodes from this room
                }
            }

            // check all nodes for neighbors
            for (int k = 0; k < tempRoomSpawnNodes.Count; k++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(tempRoomSpawnNodes[k].position, 0.5f);

                if (hitColliders.Length == 0)
                {
                    roomSpawnNodes.Add(tempRoomSpawnNodes[k]);
                }
            }
        }

        public void GenerateRooms()
        {
            generationLayer += 1;
            StartCoroutine(RoomGenWithChecks());
        }

        public void GenerateLoot()
        {
            //get loot spawn points
            for (int i = 0; i < roomsInLevel.Count; i++)
            {
                Room tempRoom = roomsInLevel[i];
                for (int j = 0; j < tempRoom.lootSpawns.Length; j++)
                {
                    allLootSpawns.Add(tempRoom.lootSpawns[j]);
                }
            }
            //randomly pick some points and instatiate loot
            for (int a = 0; a < lootPrefabs.Length; a++)
            {
                int randomIndex = UnityEngine.Random.Range(0, allLootSpawns.Count);
                Transform spawnPoint = allLootSpawns[randomIndex].transform;
                Instantiate(lootPrefabs[a], spawnPoint.position, spawnPoint.rotation);
                allLootSpawns.RemoveAt(randomIndex);
            }
        }

        public void GenerateEnemies()
        {
            //get spawn points
            for (int i = 0; i < roomsInLevel.Count; i++)
            {
                Room tempRoom = roomsInLevel[i];
                for (int j = 0; j < tempRoom.enemySpawns.Length; j++)
                {
                    allEnemySpawns.Add(tempRoom.enemySpawns[j]);
                }
            }
            //randomly pick some points and instatiate 
            for (int a = 0; a < enemyPrefabs.Length; a++)
            {
                int randomIndex = UnityEngine.Random.Range(0, allEnemySpawns.Count);
                Transform spawnPoint = allEnemySpawns[randomIndex].transform;
                Instantiate(enemyPrefabs[a], spawnPoint.position, spawnPoint.rotation);
                allEnemySpawns.RemoveAt(randomIndex);
            }
        }

        public void GenerateDoors()
        { //get doors
            for (int i = 0; i < roomsInLevel.Count; i++)
            {
                roomsInLevel[i].SpawnDoors();
            }
        }

        IEnumerator RoomGenWithChecks()
        {

            int genLyrSqr = generationLayer * generationLayer;
            Debug.Log("generation layer squared" + genLyrSqr);

            // if room spawnpoints are closer than 15 from 0,0,0, remove from list ===> after first generation
            //cull nodes based on what round of generation we're on

            if (generationLayer > 1)
            {
                for (int d = 0; d < roomSpawnNodes.Count; d++)
                {
                    if (Vector3.Distance(roomSpawnNodes[d].position, levelRooms[0].transform.position) < 15)
                    {
                        roomSpawnNodes.RemoveAt(d);

                    }
                }
                // do a switch here instead roomSpawn.Nodes.Count greater than 10 remove 4 greater than 14 remove 8
                for (int g = 0; g < generationLayer + 2; g++)
                {
                    roomSpawnNodes.RemoveAt(UnityEngine.Random.Range(0, roomSpawnNodes.Count));
                }
            }

            if (generationLayer == 1)
            {
                for (int g = 0; g < generationLayer; g++)
                {
                    roomSpawnNodes.RemoveAt(UnityEngine.Random.Range(0, roomSpawnNodes.Count));
                }
            }

            /// now that we've culled the right ab=mount of nodes. do the generation

            for (int i = 0; i < roomSpawnNodes.Count; i++)
            {
                Collider[] hitColliders = Physics.OverlapSphere(roomSpawnNodes[i].position, 0.5f); // check if there's something there already

                if (hitColliders.Length == 0)
                {
                    GameObject randomRoomPrefab = levelRooms[UnityEngine.Random.Range(0, levelRooms.Length)];
                    GameObject temp = Instantiate(randomRoomPrefab, roomSpawnNodes[i].position, Quaternion.identity);
                    temp.transform.SetParent(levelParent);
                    Room thisRoom = temp.GetComponent<Room>();
                    roomsInLevel.Add(thisRoom);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}

    





