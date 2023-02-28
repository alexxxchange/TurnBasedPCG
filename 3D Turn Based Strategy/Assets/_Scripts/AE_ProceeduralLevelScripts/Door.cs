using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class Door : MonoBehaviour
    {
        [SerializeField] GameObject doorThing;

        public void ActivateDoor()
        {
            doorThing.SetActive(true);
        }
    }
}
