using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class EndRoom : MonoBehaviour
    {
        Room thisRoom;
        // Start is called before the first frame update
        void Awake()
        {
            thisRoom = GetComponent<Room>();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}