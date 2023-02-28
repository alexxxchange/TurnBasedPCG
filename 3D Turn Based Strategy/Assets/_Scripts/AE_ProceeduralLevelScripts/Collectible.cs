using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class Collectible : MonoBehaviour
    {

        bool collected;
        [SerializeField] GameObject pickupFx;

        private void OnTriggerEnter(Collider other)
        {
            var collector = other.GetComponent<Collector>();
            if (!collected && collector != null)
            {
                collector.AddItemToCollection();
                if (pickupFx != null)
                    Instantiate(pickupFx, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
