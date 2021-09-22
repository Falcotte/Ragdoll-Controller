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
            ragdoll.GetComponent<Animator>().enabled = false;

            ragdoll.EnableRagdoll();
            ragdoll.PushRagdollComponents(explosionForce, transform.position);
        }
    }
}
