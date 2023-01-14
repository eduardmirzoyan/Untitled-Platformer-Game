using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelSetup += CenterMinimap;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelSetup -= CenterMinimap;
    }

    private void CenterMinimap(Vector2Int mapSize, List<CollectibleHandler> collectibles)
    {
        // Center camera in the middle of map
        transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2, transform.position.z);

        // Set zoom
        cam.orthographicSize = mapSize.x / 2;
    }
}
