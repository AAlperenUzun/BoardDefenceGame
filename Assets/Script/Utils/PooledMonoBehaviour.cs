using Lean.Pool;
using UnityEngine;

public class PooledMonoBehaviour : MonoBehaviour
{
    protected void Recycle()
    {
        LeanPool.Despawn(this);
    }
}