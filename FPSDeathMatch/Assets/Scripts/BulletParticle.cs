using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem bulletParticleSystem;

    List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hit");
        int events = bulletParticleSystem.GetCollisionEvents(other, colEvents);
        for (int i = 0; i < events; i++)
        { 
        
        }
    }
}
