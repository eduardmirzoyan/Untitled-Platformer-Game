using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingUI : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Vector2 scrollSpeed;
    [SerializeField] private float speedFactor = 1f;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        var newPosition = rawImage.uvRect.position + scrollSpeed * speedFactor *Time.deltaTime;
        rawImage.uvRect = new Rect(newPosition, rawImage.uvRect.size);
    }

    public void SetSpeed(float newFactor)
    {
        speedFactor = newFactor;
    }
}
