using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BombButton : MonoBehaviour
{
    [SerializeField] private Button bombButton;
    [SerializeField] private Transform bombIcon;

    [SerializeField] private AnimationCurve bombButtonAnimationCurve;

    public void PlayBombButtonAnimation()
    {
        bombButton.enabled = false;

        bombIcon.DOScale(Vector3.zero, .5f).SetEase(bombButtonAnimationCurve);
    }
}
