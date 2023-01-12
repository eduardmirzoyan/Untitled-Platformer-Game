using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Controls : ScriptableObject
{
    public KeyCode moveRightKey = KeyCode.RightArrow;
    public KeyCode moveLeftKey = KeyCode.LeftArrow;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.DownArrow;
    public KeyCode interactKey = KeyCode.UpArrow;
}
