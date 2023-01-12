using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    
    [Header("Data")]
    [SerializeField] private string currentAnimation;

    private void Awake()
    {
        // Get refs
        animator = GetComponentInChildren<Animator>();
    }

    public void ChangeAnimation(string newAnimation)
    {
        // Guard to prevent replaying same state
        if (currentAnimation == newAnimation)
            return;

        // Play new animation
        animator.Play(newAnimation);

        // Re-assign current
        currentAnimation = newAnimation;
    }

    public bool IsFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;
    }
}
