using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Camera cam;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelEnter += Center;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelEnter -= Center;
    }

    private void Center()
    {
        // Center camera in the middle of map
        transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2, transform.position.z);

        // Set zoom
        cam.orthographicSize = mapSize.x / 2; 
    }
}
