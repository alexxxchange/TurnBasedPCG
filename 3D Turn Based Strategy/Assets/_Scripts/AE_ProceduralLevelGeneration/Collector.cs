using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] int collectiblesOwned;
 

    private void Awake()
    {
        player = GetComponent<Player>(); 
    }

    public void AddItemToCollection(int amount = 1)
    {
        collectiblesOwned += amount;
    }



}
