using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class RagdollComponent : MonoBehaviour
    {
        [SerializeField] private Rigidbody componentRigidbody;
        public Rigidbody ComponentRigidbody => componentRigidbody;
        [SerializeField] private Collider componentCollider;
        public Collider ComponentCollider => componentCollider;

        private void Reset()
        {
            componentRigidbody = GetComponent<Rigidbody>();
            componentCollider = GetComponent<Collider>();
        }
    }
}