using UnityEngine;

public class CharController : MonoBehaviour
{
    [SerializeField] private Animator charAnimator;
    public Animator CharAnimator => charAnimator;
    [SerializeField] private Rigidbody charRigidbody;
    public Rigidbody CharRigidbody => charRigidbody;
    [SerializeField] private CapsuleCollider charCollider;
    public CapsuleCollider CharCollider => charCollider;
}
