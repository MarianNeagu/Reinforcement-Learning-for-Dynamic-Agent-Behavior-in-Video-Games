using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject keyPrefab;
    [SerializeField]
    private GameObject frontDoor;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private GameObject key;
    private void Start()
    {
        frontDoor.SetActive(true);
        key = Instantiate(keyPrefab, new Vector3(Random.Range(14f, 30f), 1.5f, Random.Range(-18f, 7f)), Quaternion.identity);
    }
    private void Update()
    {
        if (playerManager.getFrontDoorKeyCollected())
        {
            frontDoor.SetActive(false);
            key.SetActive(false);
        }
    }
}
