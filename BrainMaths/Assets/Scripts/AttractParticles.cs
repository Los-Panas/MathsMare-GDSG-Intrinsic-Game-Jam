using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractParticles : MonoBehaviour
{
    // The particle system to operate on
    private ParticleSystem AffectedParticles;

    private Transform target;
  
    [Range(0.0f,1.0f)]
    public float speed = 1.0f;

    // Transform cache
    // Array to store particles info
    private ParticleSystem.Particle[] m_rParticlesArray = null;
    // Is this particle system simulating in world space?


    void Awake()
    {
        target = GameObject.Find("GradeBar").transform;
        if (target == null)
        {
            Destroy(this);
            return;
        }
        AffectedParticles = GetComponent<ParticleSystem>();
        Setup();

    }

    // To store how many particles are active on each frame
    private int m_iNumActiveParticles = 0;
    // The attractor target
    private Vector3 m_vParticlesTarget = Vector3.zero;
    // A cursor for the movement interpolation
    void LateUpdate()
    {
        if (!AffectedParticles.IsAlive())
        {
            EnemiesSpawner.instance.PlayTickSFX();

            Destroy(gameObject);
        }

        // Work only if we have something to work on :)
        if (AffectedParticles != null)
        {
            m_iNumActiveParticles = AffectedParticles.GetParticles(m_rParticlesArray);

            m_vParticlesTarget = target.position;

            for (int iParticle = 0; iParticle < m_iNumActiveParticles; iParticle++)
            { // The movement cursor is the opposite of the normalized particle's lifetime m_fCursor = 1.0f - (m_rParticlesArray[iParticle].lifetime / m_rParticlesArray[iParticle].startLifetime); // Are we over the activation treshold? if (m_fCursor >= ActivationTreshold)
                {
                    if((m_rParticlesArray[iParticle].position - transform.InverseTransformPoint(m_vParticlesTarget)).magnitude < 0.5f &&
                        (m_rParticlesArray[iParticle].position - transform.InverseTransformPoint(m_vParticlesTarget)).magnitude > -0.5f)
                    {
                        m_rParticlesArray[iParticle].remainingLifetime = 0.0f;
                    }
                    // Take over the particle system imposed velocity
                    m_rParticlesArray[iParticle].velocity = Vector3.zero;
                    // Interpolate the movement towards the target with a nice quadratic easing					
                    m_rParticlesArray[iParticle].position = Vector3.Lerp(m_rParticlesArray[iParticle].position,transform.InverseTransformPoint(m_vParticlesTarget),speed * 0.1f);
                }
            }

            // Let's update the active particles
            AffectedParticles.SetParticles(m_rParticlesArray, m_iNumActiveParticles);
        }
    }

    public void Setup()
    {
        // If we have a system to setup...
        if (AffectedParticles != null)
        {
            // Prepare enough space to store particles info
            m_rParticlesArray = new ParticleSystem.Particle[AffectedParticles.main.maxParticles];
            // Is the particle system working in world space? Let's store this info
        }
    }
}

