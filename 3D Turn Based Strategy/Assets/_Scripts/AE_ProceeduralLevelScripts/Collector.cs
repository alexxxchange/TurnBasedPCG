using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class Collector : MonoBehaviour
    {
     //   [SerializeField] Player player;
        [SerializeField] int collectiblesOwned;


        public void AddItemToCollection(int amount = 1)
        {
            collectiblesOwned += amount;
        }



    }
}
