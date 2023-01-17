using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ParticleSystem runningParticles;
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem slidingParticles;

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

    public void ToggleSlide(bool state)
    {
        if (state) slidingParticles.Play();
        else slidingParticles.Stop();
    }
}
