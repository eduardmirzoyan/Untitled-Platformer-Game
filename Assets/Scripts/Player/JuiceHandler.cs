using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ParticleSystem runningParticles;
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;

    public void ToggleRunning(bool state)
    {
        if (state) runningParticles.Play();
        else runningParticles.Stop();
    }

    public void PlayJump()
    {
        jumpingParticles.Play();
    }

    public void PlayLand()
    {
        landingParticles.Play();
    }
}
