using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FloatingItemView : MonoBehaviour
{
    [SerializeField] private Image _image;

    private GridObjectTypeContainer _typeContainer;

    public void Initialize(GridObjectTypeContainer typeContainer)
    {
        _typeContainer = typeContainer;
        ResourcesController.Instance.TryGetSprite(typeContainer, out Sprite sprite);
        _image.sprite = sprite;
    }

    public void Reached(Transform goalTransform)
    {
        goalTransform.DOKill();
        goalTransform.DOPunchScale(Vector3.one * 0.2f, .25f).OnComplete(() =>
        {
            goalTransform.DOScale(Vector3.one, .25f);
        });
    }
}
