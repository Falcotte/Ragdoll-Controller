using UnityEngine;
using AngryKoala.Ragdoll;

public class RagdollDemo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Ragdoll ragdoll;

    public void EnableRagdoll()
    {
        animator.enabled = false;
        ragdoll.EnableRagdoll();
    }

    public void DisableRagdoll()
    {
        animator.enabled = true;
        ragdoll.DisableRagdoll();
    }
}
