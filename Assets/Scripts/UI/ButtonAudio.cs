using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play audio
        if (button.interactable)
            AudioManager.instance.Play("Button Hover");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable && eventData.button == PointerEventData.InputButton.Left)
        {
            AudioManager.instance.Play("Button Click");
        }
    }
}
