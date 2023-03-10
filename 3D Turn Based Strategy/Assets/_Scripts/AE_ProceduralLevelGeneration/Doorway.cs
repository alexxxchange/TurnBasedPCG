using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    [SerializeField] GameObject doorThing;

    public void ActivateDoor()
    {
        doorThing.SetActive(true);
    }
}
