using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private bool frontDoorKeyCollected { get; set; }

    public bool getFrontDoorKeyCollected()
    {
        return frontDoorKeyCollected;
    }

    private void Awake()
    {
        frontDoorKeyCollected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FrontDoorKey"))
        {
            frontDoorKeyCollected = true;
            Debug.Log("KEY");
        }
    }
}
