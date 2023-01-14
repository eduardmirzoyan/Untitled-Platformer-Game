using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] private float horizontalParallaxFactor;
    [SerializeField] private float verticalParallaxFactor;
    [SerializeField] private float buffer;

    private Vector2 startPosition;
    private float length;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        length = spriteRenderer.bounds.size.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sub
        LevelEvents.instance.onLevelEnter += Enable;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelEnter -= Enable;
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            float temp = target.position.x * (1 - horizontalParallaxFactor);

            // Calculated parallax'd displacement
            float distanceX = (target.position.x * horizontalParallaxFactor);
            float distanceY = (target.position.y * verticalParallaxFactor);

            // Calculate new position
            Vector2 desiredPos = startPosition + new Vector2(distanceX, distanceY);

            // Update background position
            transform.position = desiredPos;

            // Check if target is past left or right of thing
            if (temp > startPosition.x + length - buffer) startPosition.x += length;
            else if (temp < startPosition.x - length + buffer) startPosition.x -= length;
        }
    }

    private void Enable(Transform target)
    {
        // Set target
        this.target = target;

        // Get starting values
        startPosition = transform.position;
    }
}
