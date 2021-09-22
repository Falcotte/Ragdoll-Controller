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
                ragdollComponents[i].ComponentRigidbody.velocity = ragdollComponents[i].ComponentRigidbody.angularVelocity = Vector3.zero;

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

        // Push ragdoll components with an identical force
        public void PushRagdoll(Vector3 force)
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.AddForce(force, ForceMode.Impulse);
            }
        }

        // Push entire ragdoll with a force that originates from originPosition (simplified version of PushRagdollComponents)
        public void PushRagdoll(float force, Vector3 originPosition)
        {
            Vector3 forceVector = (transform.position - originPosition).normalized * force;
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.AddForce(forceVector, ForceMode.Impulse);
            }
        }

        // Push ragdoll components with a force that originates from originPosition
        public void PushRagdollComponents(float force, Vector3 originPosition)
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.AddForce((ragdollComponents[i].transform.position - originPosition).normalized * force, ForceMode.Impulse);
            }
        }
    }
}
