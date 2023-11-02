using UnityEngine;

public class GridPresenter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Camera _camera;

    public void Initialize(LevelData levelData)
    {
        Vector2 center = new Vector2((levelData.GridSize.x - 1) / 2f, (levelData.GridSize.y - 1) / 2f);
        _spriteRenderer.transform.position = new Vector3(center.x, center.y, 0f);
        _spriteRenderer.size = new Vector2(levelData.GridSize.x + 0.4f, levelData.GridSize.y + 0.4f);
        _camera.transform.position = new Vector3(center.x, center.y, _camera.transform.position.z);
        _camera.orthographicSize = Mathf.Max(_spriteRenderer.size.x, _spriteRenderer.size.y) + 1;
    }
}