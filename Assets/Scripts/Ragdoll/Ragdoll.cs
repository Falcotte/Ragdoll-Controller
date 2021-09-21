using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private List<RagdollComponent> ragdollComponents = new List<RagdollComponent>();
        public List<RagdollComponent> RagdollComponents => ragdollComponents;

        public void EnableRagdoll()
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.isKinematic = false;
                ragdollComponents[i].ComponentCollider.enabled = true;
            }
        }

        public void DisableRagdoll()
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.isKinematic = true;
                ragdollComponents[i].ComponentCollider.enabled = false;
            }
        }
    }
}
