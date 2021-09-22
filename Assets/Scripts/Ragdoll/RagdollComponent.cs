using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class RagdollComponent : MonoBehaviour
    {
        [SerializeField] private Rigidbody componentRigidbody;
        public Rigidbody ComponentRigidbody => componentRigidbody;
        [SerializeField] private Collider componentCollider;
        public Collider ComponentCollider => componentCollider;

        public enum BodyParts { Head, CenterMass, Arm, Leg };
        [Space]
        [SerializeField] private BodyParts bodyPart;
        public BodyParts BodyPart => bodyPart;

        [SerializeField] private LayerMask layerMask;
        public LayerMask LayerMask => layerMask;

        private void Reset()
        {
            componentRigidbody = GetComponent<Rigidbody>();
            componentCollider = GetComponent<Collider>();
        }

        public void SetBodyPart(BodyParts bodyPart)
        {
            this.bodyPart = bodyPart;
        }

        public void SetLayerMask(LayerMask layerMask)
        {
            this.layerMask = layerMask;
        }
    }
}