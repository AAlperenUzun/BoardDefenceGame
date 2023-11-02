using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class BaseGridObjectPresenter : PooledMonoBehaviour
{
    protected GridObjectTypeContainer _typeContainer;
    
    public virtual void Initialize(GridObjectPresenterData data)
    {
        _typeContainer = data.GridObjectTypeContainer;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(data.Position.x, data.Position.y);
    }

    public void Dispose()
    {
        Recycle();
    }

    public void Fall(Vector2Int position)
    {
        transform.DOKill();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.DOMoveY(position.y, GameUtility.FallSpeed).SetEase(Ease.InSine).SetSpeedBased().OnComplete(() =>
        {
            transform.DOPunchPosition(Vector3.up * .15f, .25f);
        });
    }

    public virtual void OnInteracted(bool result)
    {
        if (DOTween.IsTweening(transform)) return;

        if (!result)
        {
            transform.DOPunchRotation(Vector3.forward * 30, 0.5f);
        }
    }
}