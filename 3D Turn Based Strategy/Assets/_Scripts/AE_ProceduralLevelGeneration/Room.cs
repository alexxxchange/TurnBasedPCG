using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField]
	private Transform[] nodes;

	public Doorway[] doors;
	public LootSpawnPoint LootSpawn { get; private set; }
	public EnemySpawns[] EnemySpawns { get; private set; }
	public Transform[] Nodes => nodes;

	private void Awake()
	{
		LootSpawn = GetComponentInChildren<LootSpawnPoint>();
		EnemySpawns = GetComponentsInChildren<EnemySpawns>();
	}

	public void SpawnDoors()
	{
		for (int i = 0; i < doors.Length; i++)
		{
			Collider[] hitColliders = Physics.OverlapSphere(Nodes[i].position, 0.5f); // check if there's something there already

			if (hitColliders.Length == 0)
			{
				doors[i].ActivateDoor();
			}
		}
	}
}
