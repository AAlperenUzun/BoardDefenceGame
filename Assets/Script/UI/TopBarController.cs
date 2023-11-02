using System.Collections.Generic;
using Lean.Pool;
using TMPro;
using UnityEngine;

public class TopBarController : MonoBehaviour
{
    [SerializeField] private Transform _layout;
    [SerializeField] private TMP_Text _movesText;

    private LevelData _levelData;

    public void Initialize(LevelData levelData)
    {
        _levelData = levelData;
        UpdateMoveText();

        _levelData.MoveUsed += UpdateMoveText;
    }
    


    private void UpdateMoveText() => _movesText.text = _levelData.MoveCount.ToString();
}