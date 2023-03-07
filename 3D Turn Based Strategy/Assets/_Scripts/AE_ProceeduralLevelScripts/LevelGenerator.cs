using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System.Linq;



public class LevelGenerator : MonoBehaviour
{

	[Header("options")]
	[SerializeField] bool spawnDoors;
	[SerializeField] bool spawnEnemies;
	[SerializeField] bool spawnLoot;
	[SerializeField] int targetNumberOfRooms = 12;

	[Header("level Generation")]
	[SerializeField] GameObject startRoom;
	[SerializeField] GameObject[] levelRooms;
	[SerializeField] GameObject endRoomPrefab;
	[SerializeField] List<Doorway> allDoors;
	[SerializeField] List<Room> roomsInLevel;
	[SerializeField] List<Transform> roomSpawnNodes;
	[Header("loot")]
	[SerializeField] GameObject[] lootPrefabs;
	[SerializeField] List<LootSpawnPoint> allLootSpawns;

	[Header("enemies")]
	[SerializeField] GameObject[] enemyPrefabs;
	[SerializeField] List<EnemySpawns> allEnemySpawns;


	Room endRoomCached;
	int generationLayer = 0; //number of times we've iterated


	private void Start()
	{
		StartCoroutine(GenerateLevel());
	}

	IEnumerator GenerateLevel()
	{
		Debug.Log("Generating Rooms...");
		GenerateStartRoom();
		yield return new WaitForEndOfFrame();
		while (roomsInLevel.Count < targetNumberOfRooms)
		{
			GetNodes();
			yield return new WaitForEndOfFrame();
			GenerateRooms();
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(1f);

		GetNodes();
		yield return new WaitForEndOfFrame();

		GenerateEndRoom();
		yield return new WaitForSeconds(1f);

		Debug.Log("All Rooms Generated!");

		if (spawnDoors)
		{
			Debug.Log("Activating Doors...");
			GenerateDoors();
			yield return new WaitForSeconds(1f);
		}

		// Adding pathfinding setup after room generation fixes issue with walls being walkable

		Debug.Log("Generating Pathfinding Grid...");
		Pathfinding.Instance.Setup(LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight(), LevelGrid.Instance.GetCellSize());

		yield return new WaitForSeconds(1f);

		Debug.Log("initilizing Grid visuals...");
		GridSystemVisual.Instance.InitGridVisuals();

		if (spawnEnemies)
		{
			Debug.Log("Spawning Enemies...");
			GenerateEnemies();
			yield return new WaitForSeconds(1f);
		}

		if (spawnLoot)
		{
			Debug.Log("Spawning Loot...");
			GenerateLoot();
			yield return new WaitForSeconds(1f);
		}

		

		Debug.Log("Level Setup Complete!");
	}

	private void GenerateEndRoom()
	{
		Transform lastRoomSpawn = roomSpawnNodes.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).Last();
		GameObject temp = Instantiate(endRoomPrefab, lastRoomSpawn.position, Quaternion.identity);
		Room thisRoom = temp.GetComponent<Room>();
		roomsInLevel.Add(thisRoom); // maybe maybe not?... in case we want to do anything with the filler rooms at runtime like lock all doors etc...

		endRoomCached = thisRoom;
	}

	public void GenerateStartRoom()
	{
		GameObject temp = Instantiate(startRoom, new Vector3(100, 0, 100), Quaternion.identity); // the "new Vector3" needs to be changed dynamically for larger dungeons. i.e. moved farther from 0,0.
		Room thisRoom = temp.GetComponent<Room>();
		roomsInLevel.Add(thisRoom);
	}

	private void GetNodes()
	{
		roomSpawnNodes.Clear();// clear list
		List<Transform> tempRoomSpawnNodes = new List<Transform>(); // create temp list

		for (int i = 0; i < roomsInLevel.Count; i++) // add all rooms to temp list
		{
			Room tempRoom = roomsInLevel[i]; // add all nodes from this room
			for (int j = 0; j < tempRoom.nodes.Length; j++)
			{
				tempRoomSpawnNodes.Add(tempRoom.nodes[j]); //add each to temp list
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


		for (int i = 0; i < roomSpawnNodes.Count; i++)
		{
			Collider[] hitColliders = Physics.OverlapSphere(roomSpawnNodes[i].position, 0.5f); // check if there's something there already

			if (hitColliders.Length == 0)
			{
				GameObject randomRoomPrefab = levelRooms[UnityEngine.Random.Range(0, levelRooms.Length)];
				GameObject temp = Instantiate(randomRoomPrefab, roomSpawnNodes[i].position, Quaternion.identity);
				Room thisRoom = temp.GetComponent<Room>();
				roomsInLevel.Add(thisRoom);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}







