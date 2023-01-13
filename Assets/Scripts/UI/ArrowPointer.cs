using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPointer : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera cam;
    [SerializeField] private Image pointerImage;
    [SerializeField] private RectTransform pointerRectTransform;

    [Header("Data")]
    [SerializeField] private Transform fromTarget;
    [SerializeField] private Transform toTarget;

    [Header("Settings")]
    [SerializeField] private float borderSize = 100f;
    [SerializeField] private float minDistance = 10f;

    private void Awake()
    {
        // Set cam
        cam = Camera.main;
    }

    public void Initialize(Transform fromTarget, Transform toTarget, Color color)
    {
        this.toTarget = toTarget;
        this.fromTarget = fromTarget;
        pointerImage.color = color;
    }

    public void Update()
    {
        // If any target is lost
        if (fromTarget == null || toTarget == null)
        {
            // Destroy this
            Destroy(gameObject);
            return;
        }

        // Check if the target is off screen or not
        Vector3 targetPositionScreenPoint = cam.WorldToScreenPoint(toTarget.position);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        var worldPos = pointerRectTransform.transform.position;
        worldPos.z = 0f;

        // If off screen and not too close
        if (isOffScreen && Vector3.Distance(worldPos, toTarget.position) > minDistance)
        {
            // Rotate the pointer so it points tward it
            RotatePointerTowardsTargetPosition();

            // Show arrow image
            pointerImage.enabled = true;

            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
            cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

            Vector3 pointerWorldPosition = cam.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }
        else
        {
            // Disable image
            pointerImage.enabled = false;
        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        Vector3 toPosition = toTarget.position;
        Vector3 fromPosition = fromTarget.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        // Point towards target
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointerRectTransform.transform.position, minDistance);
    }
}