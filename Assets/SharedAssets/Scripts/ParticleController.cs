using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // Event for particle collision
    public delegate void ParticleCollisionHandler(GameObject particle, Collision collision);
    public event ParticleCollisionHandler OnParticleCollision;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if there are any subscribers to the event
        if (OnParticleCollision != null)
        {
            // Invoke the event, passing the particle and collision information
            OnParticleCollision(gameObject, collision);
        }
    }
}
