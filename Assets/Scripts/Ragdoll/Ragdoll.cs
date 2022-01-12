using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private List<RagdollComponent> ragdollComponents = new List<RagdollComponent>();
        public List<RagdollComponent> RagdollComponents => ragdollComponents;

        /// <summary>
        /// Enables the ragdoll by its components
        /// </summary>
        public void EnableRagdoll()
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.isKinematic = false;
                ragdollComponents[i].ComponentRigidbody.velocity = ragdollComponents[i].ComponentRigidbody.angularVelocity = Vector3.zero;

                ragdollComponents[i].ComponentCollider.enabled = true;
            }
        }

        /// <summary>
        /// Disables the ragdoll by its components
        /// </summary>
        public void DisableRagdoll()
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.isKinematic = true;
                ragdollComponents[i].ComponentCollider.enabled = false;
            }
        }

        /// <summary>
        /// Push each ragdoll component individually with with an identical force
        /// </summary>
        /// <param name="force"></param>
        public void PushRagdoll(Vector3 force)
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.AddForce(force, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Push each ragdoll component individually with a newly calculated force that originates from originPosition
        /// </summary>
        /// <param name="force"></param>
        /// <param name="originPosition"></param>
        public void PushRagdoll(float force, Vector3 originPosition)
        {
            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                Vector3 forceVector = (ragdollComponents[i].transform.position - originPosition).normalized * force;
                ragdollComponents[i].ComponentRigidbody.AddForce(forceVector, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Push each ragdoll component individually with with an identical force that originates from originPosition (simplified version of PushRagdollComponents)
        /// </summary>
        /// <param name="force"></param>
        /// <param name="originPosition"></param>
        public void PushRagdollUniform(float force, Vector3 originPosition)
        {
            Vector3 forceVector = (transform.position - originPosition).normalized * force;

            for(int i = 0; i < ragdollComponents.Count; i++)
            {
                ragdollComponents[i].ComponentRigidbody.AddForce(forceVector, ForceMode.Impulse);
            }
        }
    }
}
