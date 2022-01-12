using System.Collections.Generic;
using UnityEngine;
using AngryKoala.Ragdoll;

public class Bomb : MonoBehaviour
{
    [SerializeField] private List<Ragdoll> ragdolls;

    [SerializeField] private float explosionForce;

    public void Explode()
    {
        foreach(var ragdoll in ragdolls)
        {
            // Ideally the object should have a reference to the animator component 
            ragdoll.GetComponent<Animator>().enabled = false;

            ragdoll.EnableRagdoll();
            ragdoll.PushRagdoll(explosionForce, transform.position);
        }
    }
}
